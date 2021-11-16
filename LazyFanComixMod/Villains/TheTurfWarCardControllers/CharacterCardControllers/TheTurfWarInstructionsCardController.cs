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
            this.SpecialStringMaker.ShowSpecialString(() => "This should eventually track how many cards are under each leader.");
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


        // TODO: Verify this is necessary.
        //public override void AddDependentSpecialStrings()
        //{
        //    Card card = this.TurnTaker.FindCard("TheShrineOfTheEnnead", true);
        //    if (card != null)
        //    {
        //        this.SpecialStringMaker.ShowNumberOfCardsUnderCard(card, null);
        //    }
        //}

        public override void AddSideTriggers()
        {
            // Instant win at 0 Villain Leaders True for both sides.
            this.AddSideTrigger(
                this.AddTrigger(
                    delegate (GameAction g)
                    {
                        if (this.GameController.HasGameStarted && !(g is GameOverAction) && !(g is MessageAction) && !(g is IncrementAchievementAction))
                        {
                            return this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("leader")).Count() == 0;
                        }
                        return false;
                    },
                (GameAction g) => WinGameFull(g), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After)
            );


            if (!this.Card.IsFlipped)
            {
                // On front: Start of Turn, win if only one villain leader remains.
                this.AddSideTrigger(
                    this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction p) => WinGamePartial(p), new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, (PhaseChangeAction p) => this.FindCardsWhere((Card c) => c.IsVillain && c.DoKeywordsContain("leader")).Count() == 1)
                );

                // On front: End of Turn, flip if there are any villain leader cards with at least 4 cards under it.
                this.AddSideTrigger(
                    this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction p) => this.GameController.FlipCard(this, cardSource: this.GetCardSource()), TriggerType.FlipCard, (PhaseChangeAction p) => this.FindCardsWhere((Card c) => c.IsVillain && c.DoKeywordsContain("leader") && c.UnderLocation.Cards.Count() >= 4).Count() > 0)
                );

            }
            else
            {
                // On back: Start of Turn, large overhaul of actions.
                this.AddSideTrigger(
                    this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => VillainPlot(pca),
                    new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage, TriggerType.RevealCard, TriggerType.PlayCard, TriggerType.FlipCard })
                );
            }
        }

        // Token: 0x06002A5B RID: 10843 RVA: 0x0003A06F File Offset: 0x0003826F
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == this.Card;
        }

        private IEnumerator WinGamePartial(GameAction ga)
        {
            IEnumerator coroutine;

            Card c = this.GameController.FindCardsWhere((Card c) => c.IsVillain && c.DoKeywordsContain("leader")).FirstOrDefault();

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

        private IEnumerator VillainPlot(GameAction ga)
        {
            IEnumerator coroutine;
            List<Card> underDestroyed = new List<Card>();

            //// Convoluted TODO: SelectCardsAndDoAction with appropriate criteria to actively track what has already had two cards under it selected! Do later, since this is a massive headache!
            //// First criteria to find cards that we'll check under: Is a leader and hasn't already had two cards destroyed from under it. 
            //LinqCardCriteria leaderAndNotTwoDestroyed = new LinqCardCriteria((Card c) => c.DoKeywordsContain("leader") && underDestroyed.Where((Card c2) => c == c2).Count() < 2);
            //// Second criteria to find cards: Look at the list of leaders, and see if the location of the card matches the 'under' location of each of those.
            //Func<Card, bool> underCriteria = (Card c) => this.GameController.FindCardsWhere(leaderAndNotTwoDestroyed).Select((Card c2) => c2.UnderLocation).Contains(c.Location);

            //SelectCardsDecision scsd = new SelectCardsDecision(this.GameController, this.DecisionMaker, underCriteria, SelectionType.DestroyCard, null, false, null, true, false, false, () => DoesUnderDestructionContinue(underDestroyed), cardSource: this.GetCardSource());
            //coroutine = this.GameController.SelectCardsAndDoAction(scsd, (SelectCardDecision sc) => this.DestroyAndTrackBaseCard(sc, underDestroyed), null, null, this.GetCardSource());
            //if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            // Less convoluted TODO: Custom SelectCardAndDoAction for "Destroy 2 cards from under each Leader", since tracking through SelectCardsAndDoAction is nightmarish. 
            coroutine = DestroyTwoFromUnderEachLeader();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Look over all leaders for the one with the most cards. There doesn't appear to be a valid method of using TargetInfo for this.
            IEnumerable<Card> leaders = this.FindCardsWhere((Card c) => c.IsVillain && c.DoKeywordsContain("leader"));
            int maxCards = leaders.Select((Card c) => c.UnderLocation.Cards.Count()).Max();
            leaders = leaders.Where((Card c) => c.UnderLocation.Cards.Count() == maxCards);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, leaders);

            // Selected villain damages and, if legal after, discovers a matching card.
            coroutine = this.GameController.SelectCardAndDoAction(scd, DealDamageAndDiscoverCard);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            // Flip back.
            coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        //private int DoesUnderDestructionContinue(List<Card> underDestroyed)
        //{
        //    // First criteria to find cards that we'll check under: Is a leader and hasn't already had two cards destroyed from under it. 
        //    LinqCardCriteria leaderAndNotTwoDestroyed = new LinqCardCriteria((Card c) => c.DoKeywordsContain("leader") && underDestroyed.Where((Card c2) => c == c2).Count() < 2);

        //    // When considering how much is left to destroy from under remaining cards - for each remaining leader, start with 2 and subtract what's already been destroyed from under it. Or, if fewer cards remain, every cards that remains.
        //    Func<Card, int> leftToDestroyFromUnder = (Card c) => Math.Min(2 - underDestroyed.Where((Card c2) => c == c2).Count(), c.UnderLocation.Cards.Count());

        //    int moreToDestroy = this.GameController.FindCardsWhere(leaderAndNotTwoDestroyed).Sum(leftToDestroyFromUnder);

        //    return underDestroyed.Count() + moreToDestroy;
        //}

        //private IEnumerator DestroyAndTrackBaseCard(SelectCardDecision scd, List<Card> underDestroyed)
        //{
        //    IEnumerator coroutine;
        //    if (scd!.SelectedCard != null)
        //    {
        //        underDestroyed.Add(scd.SelectedCard);
        //        coroutine = this.GameController.DestroyCard(this.DecisionMaker, scd.SelectedCard, cardSource: this.GetCardSource());
        //        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        //    }
        //}

        private IEnumerator DestroyTwoFromUnderEachLeader()
        {
            List<Card> leadersUnderDestroyAttempts = new List<Card>();
            List<Card> cardFailedDestroyAttempts = new List<Card>();
            bool continueChecking = true;
            IEnumerator coroutine;

            // TODO: Requires clearing leaders array if that leader is destroyed, and card array if card is destroyed. Hope that never happens, but it might. Likely uses temporary triggers to manage. Yes, this is a pain. I think Play Indexes can technically get around this, if I can look up how to use those? 

            // In-line function - each leader should have a remaining destroy count of 2 minus the number destroyed, or
            // The number of cards remaining under that have not yet had an attempted destroy, whichever is smaller. 
            Func<Card, int> destroyFromUnderRemaining = (Card c) =>
            {
                return Math.Min(
                    2 - leadersUnderDestroyAttempts.Where((Card c2) => c == c2).Count(),
                    c.UnderLocation.Cards.Where((Card c2) => !cardFailedDestroyAttempts.Contains(c2)).Count()
                );
            };

            // Must re-fetch, so track separately. 
            while (continueChecking)
            {
                // TODO: Should likely be SelectCardAndDoAction scenario, especially since we need to check for validity prior to doing action to prevent unnecessary message.
                List<SelectCardDecision> scd = new List<SelectCardDecision>();
                // TODO: Can the efficiency of this be improved through swapping to using card.location.isundercard for various checks? 
                IEnumerable<Location> locationsUnderValidLeaders = this.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("leader") && c.IsInPlayAndHasGameText && destroyFromUnderRemaining(c) > 0)).Select((Card c) => c.UnderLocation);


                // TO DO: This should be a SelectCardAndDoAction. 
                //IEnumerator coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => locationsUnderValidLeaders.Contains(c.Location)), 1, false, 1, storedResultsAction: dca, cardSource: this.GetCardSource());

                SelectCardsDecision selectFromValidUnderCards = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => locationsUnderValidLeaders.Contains(c.Location), SelectionType.DestroyCard, 1, false, 1, cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(selectFromValidUnderCards, (SelectCardDecision scd) => IfCardSelectedTrackAndDestroy(scd, leadersUnderDestroyAttempts, cardFailedDestroyAttempts), scd, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (scd?.FirstOrDefault()?.SelectedCard == null)
                {
                    // TODO: Add initial check if all cards are destroyed/when cards are destroyed? Is it even worth doing?
                    continueChecking = false;
                }
            }
            yield break;
        }

        private IEnumerator IfCardSelectedTrackAndDestroy(SelectCardDecision scd, List<Card> leadersUnderDestroyAttempts, List<Card> cardFailedDestroyAttempts)
        {
            IEnumerator coroutine;
            if (scd?.SelectedCard != null && scd.SelectedCard.Location.IsUnderCard)
            {
                List<DestroyCardAction> dca = new List<DestroyCardAction>();
                // Get card this card is under. 
                leadersUnderDestroyAttempts.Add(scd.SelectedCard.Location.OwnerCard);

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
            Card leader = scd.SelectedCard;
            IEnumerator coroutine;
            if (leader != null)
            {
                // Damaging aspects. 
                coroutine = this.GameController.DealDamage(this.DecisionMaker, leader, (Card c) => c != leader && c.DoKeywordsContain("leader"), 2, DamageType.Toxic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DealDamage(this.DecisionMaker, leader, (Card c) => !c.DoKeywordsContain("leader") && !SharesKeyword(leader, c), 2, DamageType.Toxic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Discover a shared keyword. 
                // TODO: NEEDS TEST FOR IF THE LEADER IS INCAPPED AND HAS THEREFORE LOST KEYWORDS!
                coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => SharesKeyword(leader, c)), 1);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private bool SharesKeyword(Card leader, Card c)
        {
            // It shares keywords, so this fails. 
            foreach (string keyword in leader.GetKeywords())
            {
                if (c.DoKeywordsContain(keyword))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
