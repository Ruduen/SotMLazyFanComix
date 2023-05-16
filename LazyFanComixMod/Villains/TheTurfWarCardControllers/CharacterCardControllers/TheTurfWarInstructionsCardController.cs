using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class TheTurfWarInstructionsCardController : VillainCharacterCardController
    {
        public TheTurfWarInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // TODO: See string!
            this.SpecialStringMaker.ShowSpecialString(() => "This should eventually track how many cards are under each figurehead.");
            this.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddStartOfGameTriggers()
        {
            // Special - Set up this way rather than using TurnTakers for compatibility sake.
            SetupCharacters();
        }

        private void SetupCharacters()
        {
            List<Card> cardsToMove = new List<Card>();
            foreach (Card c in this.TurnTaker.OffToTheSide.Cards)
            {
                switch (c.Identifier)
                {
                    case "Calin":
                    case "Kanya":
                    case "Sez":
                        cardsToMove.Add(c);
                        break;

                    default:
                        break;
                }
            }

            foreach (Card c in cardsToMove)
            {
                this.TurnTaker.MoveCard(c, this.TurnTaker.PlayArea);
            }
        }

        public override void AddSideTriggers()
        {
            // Instant win at 0 Villain Figureheads True for both sides.
            this.AddSideTrigger(
                this.AddTrigger(
                    delegate (GameAction g)
                    {
                        if (this.GameController.HasGameStarted && !(g is GameOverAction) && !(g is MessageAction) && !(g is IncrementAchievementAction))
                        {
                            return this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead")).Count() == 0;
                        }
                        return false;
                    },
                (GameAction g) => WinGameFull(g), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After)
            );

            if (!this.Card.IsFlipped)
            {
                // On front: Start of Turn, win if only one villain figurehead remains.
                this.AddSideTrigger(
                    this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction p) => WinGamePartial(p), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, (PhaseChangeAction p) => this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead")).Count() == 1)
                );

                // On front: End of Turn, flip if there are any villain figurehead cards with at least 4 cards under it.
                this.AddSideTrigger(
                    this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction p) => this.GameController.FlipCard(this, cardSource: this.GetCardSource()), TriggerType.FlipCard, (PhaseChangeAction p) => this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead") && c.UnderLocation.Cards.Count() >= 4).Count() > 0)
                );

                // On front: When environment cards are destroyed, put it under the appropriate figurehead.
                this.AddSideTrigger(
                    this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.IsEnvironment && !dca.CardToDestroy.Card.Location.IsUnderCard && dca.PostDestroyDestinationCanBeChanged && dca.CardToDestroy.CanBeDestroyed && dca.WasCardDestroyed, PutUnderAFigurehead, TriggerType.MoveCard, TriggerTiming.After)
                );
            }
            else
            {
                // On back: Start of Turn, large number of actions.
                this.AddSideTrigger(
                    this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => VillainPlot(pca),
                    new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage, TriggerType.RevealCard, TriggerType.PlayCard, TriggerType.FlipCard })
                );
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == this.Card;
        }

        private IEnumerator WinGamePartial(GameAction ga)
        {
            IEnumerator coroutine;

            Card c = this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead")).FirstOrDefault();

            coroutine = this.GameController.SendMessageAction("The war is over, but " + c.Title + " will now be watching over " + this.GameController.FindEnvironmentTurnTakerController().Name + "...", Priority.Critical, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.DefeatedResponse(ga);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator WinGameFull(GameAction ga)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SendMessageAction("The war is over, and " + this.GameController.FindEnvironmentTurnTakerController().Name + " has been cleared of all villainous influence!", Priority.Critical, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.DefeatedResponse(ga);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator PutUnderAFigurehead(DestroyCardAction dca)
        {
            IEnumerator coroutine;

            // Look over all figureheads for the one with the most shared keywords. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));

            Dictionary<Card, int> matches = new Dictionary<Card, int>();
            int maxMatches = -1;
            foreach (Card c in figureheads)
            {
                int match = this.FindCardsWhere((Card c2) => c2.IsInPlayAndHasGameText && !c2.DoKeywordsContain("figurehead") && c2.DoKeywordsContain(c.GetKeywords())).Count();
                matches.Add(c, match);
                maxMatches = Math.Max(match, maxMatches);
            }

            figureheads = matches.Where((KeyValuePair<Card, int> pair) => pair.Value == maxMatches).Select((KeyValuePair<Card, int> pair) => pair.Key);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCardToUnderCard, figureheads);

            // Selected villain damages and, if legal, discovers a matching card.
            coroutine = this.GameController.SelectCardAndDoAction(scd, (SelectCardDecision scd2) => PutDestroyedCardUnder(scd2, dca));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator PutDestroyedCardUnder(SelectCardDecision scd, DestroyCardAction dca)
        {
            if (scd?.SelectedCard != null)
            {
                dca.SetPostDestroyDestination(scd.SelectedCard.UnderLocation, cardSource: this.GetCardSource());
            }
            yield return null;
        }

        private IEnumerator VillainPlot(GameAction ga)
        {
            IEnumerator coroutine;
            List<Card> underDestroyed = new List<Card>();

            // Look over all figureheads for the ones with the most cards. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));

            Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
            int cardCountMax = -1;
            foreach (Card c in figureheads)
            {
                int cardCount = c.UnderLocation.Cards.Count();
                cardCounts.Add(c, cardCount);
                cardCountMax = Math.Max(cardCountMax, cardCount);
            }

            figureheads = cardCounts.Where((KeyValuePair<Card, int> pair) => pair.Value == cardCountMax).Select((KeyValuePair<Card, int> pair) => pair.Key);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, figureheads);

            // Selected villain damages and, if legal, discovers a matching card.
            coroutine = this.GameController.SelectCardAndDoAction(scd, DestroyCardsDealDamageAndDiscoverCard);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Flip back.
            coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyCardsDealDamageAndDiscoverCard(SelectCardDecision scd)
        {
            Card figurehead = scd.SelectedCard;
            IEnumerator coroutine;
            if (figurehead != null)
            {
                // Destroy 3 cards from under.
                coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == figurehead.UnderLocation, "under " + figurehead.Title, false, true), 3, false, 3, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Damaging aspects.
                coroutine = this.GameController.DealDamage(this.DecisionMaker, figurehead, (Card c) => c != figurehead && c.DoKeywordsContain("figurehead"), 2, DamageType.Toxic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DealDamage(this.DecisionMaker, figurehead, (Card c) => !c.DoKeywordsContain("figurehead") && !c.DoKeywordsContain(figurehead.GetKeywords()), 2, DamageType.Toxic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Discover a shared keyword.
                coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain(figurehead.GetKeywords())), 1);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}