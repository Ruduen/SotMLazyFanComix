using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheTurfWarInstructionsCardController : VillainCharacterCardController
    {
        // Token: 0x06002A58 RID: 10840 RVA: 0x0006B624 File Offset: 0x00069824
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
                    this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) =>dca.CardToDestroy.Card.IsEnvironment && !dca.CardToDestroy.Card.Location.IsUnderCard &&dca.PostDestroyDestinationCanBeChanged && dca.CardToDestroy.CanBeDestroyed && dca.WasCardDestroyed, PutUnderAFigurehead, TriggerType.MoveCard, TriggerTiming.After)
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
            IEnumerator coroutine;
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

            // Destroy 2 cards from under each figurehead.
            coroutine = DestroyTwoFromUnderEachFigurehead();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            // Look over all figureheads for the ones with the most cards. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));

            Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
            int cardCountMax = -1;
            foreach (Card c in figureheads)
            {
                int cardCount = c.UnderLocation.Cards.Count();
                cardCounts.Add(c, cardCount);
                cardCountMax = Math.Max(cardCount, cardCount);
            }

            figureheads = cardCounts.Where((KeyValuePair<Card, int> pair) => pair.Value == cardCountMax).Select((KeyValuePair<Card, int> pair) => pair.Key);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, figureheads);

            // Selected villain damages and, if legal, discovers a matching card.
            coroutine = this.GameController.SelectCardAndDoAction(scd, DealDamageAndDiscoverCard);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Flip back.
            coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyTwoFromUnderEachFigurehead()
        {
            List<Card> figureheadsUnderDestroyAttempts = new List<Card>();
            List<Card> cardFailedDestroyAttempts = new List<Card>();
            bool continueChecking = true;
            IEnumerator coroutine;

            // TODO: Requires clearing figureheads array if that figurehead is destroyed, and card array if card is destroyed. Hope that never happens, but it might. Likely uses temporary triggers to manage. Yes, this is a pain. I think Play Indexes can technically get around this, if I can look up how to use those, but that would require separate tracking. That won't work for figureheads flipping, so I probably don't bother with that - not the right timing points for that to happen midway.

            // In-line function - each figurehead should have a remaining destroy count of 2 minus the number destroyed, or
            // The number of cards remaining under that have not yet had an attempted destroy, whichever is smaller. 
            Func<Card, int> destroyFromUnderRemaining = (Card c) =>
            {
                return Math.Min(
                    2 - figureheadsUnderDestroyAttempts.Where((Card c2) => c == c2).Count(),
                    c.UnderLocation.Cards.Where((Card c2) => !cardFailedDestroyAttempts.Contains(c2)).Count()
                );
            };

            // Must re-fetch, so track separately. 
            while (continueChecking)
            {
                // SelectCardAndDoAction may be slightly more efficient, but doesn't cleanly allow for CardSources if possibble, so try not to use that.

                List<SelectCardDecision> scd = new List<SelectCardDecision>();
                IEnumerable<Card> validFigureheads = this.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("figurehead") && destroyFromUnderRemaining(c) > 0));

                // No more remaining valid figureheads to destroy from under, so abort.
                // Should we have a message for if there are no cards to destroy? Technically possible with flip -> environment destruction... But also, wow.
                if (validFigureheads.Count() == 0)
                {
                    continueChecking = false;
                }

                SelectCardsDecision selectFromValidUnderCards = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.Location.IsUnderCard && validFigureheads.Contains(c.Location.OwnerCard), SelectionType.DestroyCard, 1, false, 1, cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(selectFromValidUnderCards, (SelectCardDecision scd) => IfCardSelectedTrackAndDestroy(scd, figureheadsUnderDestroyAttempts, cardFailedDestroyAttempts), scd, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // We somehow failed to select a card, so something weird happened. Abort.
                if (scd?.FirstOrDefault()?.SelectedCard == null)
                {
                    continueChecking = false;
                }
            }
            yield break;
        }

        private IEnumerator IfCardSelectedTrackAndDestroy(SelectCardDecision scd, List<Card> figureheadsUnderDestroyAttempts, List<Card> cardFailedDestroyAttempts)
        {
            IEnumerator coroutine;
            if (scd?.SelectedCard != null && scd.SelectedCard.Location.IsUnderCard)
            {
                List<DestroyCardAction> dca = new List<DestroyCardAction>();
                // Get card this card is under. 
                figureheadsUnderDestroyAttempts.Add(scd.SelectedCard.Location.OwnerCard);

                // Actually destroy.
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, scd.SelectedCard, storedResults: dca, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // If a card was selected but destruction failed, add it to the list.
                if (dca?.FirstOrDefault()?.CardToDestroy?.Card != null)
                {
                    if (!dca.FirstOrDefault().WasCardDestroyed)
                    {
                        cardFailedDestroyAttempts.Add(dca.FirstOrDefault().CardToDestroy.Card);
                    }
                }
            }
        }

        private IEnumerator DealDamageAndDiscoverCard(SelectCardDecision scd)
        {
            Card figurehead = scd.SelectedCard;
            IEnumerator coroutine;
            if (figurehead != null)
            {
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
