using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.MissInformation
{
    public class MissInformationSuspectSecretaryCharacterCardController : VillainCharacterCardController
    {

        public MissInformationSuspectSecretaryCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return (card.IsVillainTarget && !this.Card.IsFlipped);
        }

        public override void AddSideTriggers()
        {
            this.AddAsPowerContributor();

            if (!this.Card.IsFlipped)
            {
                // Indestructible check handled in other settings. 

                // After a villain would be reduced to 0 HP, restore it to max and up to 3 players may use a power. 
                this.AddSideTrigger(this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target.IsVillainTarget && dda.Amount >= dda.Target.HitPoints.Value, RestoreTargetAndHeroPowersResponse, new TriggerType[] { TriggerType.UsePower }, TriggerTiming.Before));

                // Power checks handled in other settings. 

                // At the start of the villain turn, if there are enough card, flip.. 
                this.AddSideTrigger(this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, FlipResponse, TriggerType.FlipCard));
            }
            else
            {
                this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker,
                    (PhaseChangeAction pca) => this.DealDamage(this.CharacterCard, (Card c) => c.IsHero, 3, DamageType.Psychic),
                    TriggerType.DealDamage)
                );
            }
            this.AddDefeatedIfDestroyedTriggers();
        }
        private IEnumerator RestoreTargetAndHeroPowersResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;

            // Cancel destruction, restore to max HP.
            coroutine = this.CancelAction(dda);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.IsRealAction(dda))
            {
                coroutine = this.GameController.SetHP(dda.Target, dda.Target.MaximumHitPoints.Value, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Up to 3 players may use a power. 
                coroutine = this.GameController.SelectTurnTakersAndDoAction(this.DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.UsePower, (TurnTaker tt) => this.GameController.SelectAndUsePower(this.GameController.FindHeroTurnTakerController(tt.ToHero()), true, cardSource: this.GetCardSource()), 3, false, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {

            // Do basic stuff. 
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<RevealCardsAction> rcaResults = new List<RevealCardsAction>();
            List<MoveCardAction> mcaResults = new List<MoveCardAction>();

            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsVillainTarget, (Card c) => c.MaximumHitPoints.Value - c.HitPoints.Value, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.ChangeMaximumHP(this.Card, this.Card.Definition.FlippedHitPoints.Value, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.RevealCards(this.TurnTakerController, this.Card.UnderLocation, (Card c) => false, 1, rcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            Card[] revealedCards = rcaResults?.FirstOrDefault()?.RevealedCards.ToArray();
            if (revealedCards != null)
            {
                Card[] clues = revealedCards.Where((Card c) => c.IsClue).ToArray();
                Card[] diversions = revealedCards.Where((Card c) => c.IsDiversion).ToArray();
                Card[] other = revealedCards.Except(clues).Except(diversions).ToArray();

                coroutine = this.GameController.MoveCards(this.TurnTakerController, clues, this.Card.UnderLocation, playIfMovingToPlayArea: false, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCards(this.TurnTakerController, diversions, this.TurnTaker.PlayArea, false, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCards(this.TurnTakerController, other, this.TurnTaker.Trash, isDiscard: true, storedResultsAction: mcaResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Cleanup if something weird happened.
                coroutine = this.CleanupRevealedCards(this.TurnTaker.Revealed, this.Card.UnderLocation);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.DealDamage(this.CharacterCard, (Card c) => c.IsHero, mcaResults.Where((MoveCardAction mca) => mca.WasCardMoved).Count(), DamageType.Psychic);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        private IEnumerator FlipResponse(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            if (this.Card.UnderLocation.Cards.Count() >= (this.H * 2))
            {
                coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DestroyAnyCardsThatShouldBeDestroyed(cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && cardController.Card.Owner.IsHero && !cardController.Card.Owner.ToHero().IsIncapacitatedOrOutOfGame && !cardController.Card.IsFlipped)
            {
                if (!this.Card.IsFlipped)
                {
                    return new Power[] {
                        // Power 1: Place top or bottom card.
                        new Power(cardController.HeroTurnTakerController, cardController, "Place the top or bottom card of the Villain deck under {MissInformation}.",PlaceVillainCardResponse(cardController), 0, null, this.GetCardSource()),
                        // Power 2: Look at bottom, return or discard.
                        new Power(cardController.HeroTurnTakerController, cardController, "Reveal the bottom card of the Villain deck. Put it on the bottom of the Villain deck or in the Villain trash.",ControlBottomCardResponse(cardController), 1, null, this.GetCardSource())
                    };
                }
                else
                {
                    return new Power[] {
                        new Power(cardController.HeroTurnTakerController, cardController, "Destroy " + (this.H - 1) + " cards under {MissInformation}. If 2 cards are destroyed this way, destroy a Diversion or Ongoing card.", DestroyCardsUnderResponse(cardController), 0, null, this.GetCardSource())
                    };
                }
            }
            return null;
        }

        private IEnumerator PlaceVillainCardResponse(CardController cardController)
        {
            Function[] options = new Function[] {
                new Function(cardController.HeroTurnTakerController,"Move the top card of the Villain deck",SelectionType.MoveCardToUnderCard,()=>this.GameController.MoveCard(cardController.TurnTakerController, this.TurnTaker.Deck.TopCard, this.CharacterCard.UnderLocation, playCardIfMovingToPlayArea: false, showMessage: true, cardSource: this.GetCardSource()),this.TurnTaker.Deck.HasCards),
                new Function(cardController.HeroTurnTakerController,"Move the bottom card of the Villain deck",SelectionType.MoveCardToUnderCard,()=>this.GameController.MoveCard(cardController.TurnTakerController, this.TurnTaker.Deck.BottomCard, this.CharacterCard.UnderLocation, playCardIfMovingToPlayArea: false, showMessage: true, cardSource: this.GetCardSource()),this.TurnTaker.Deck.HasCards)
            };

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, cardController.HeroTurnTakerController, options, false, null, "The villain deck does not have any cards, so no cards can be moved.", cardSource: this.GetCardSource());

            IEnumerator coroutine = this.GameController.SelectAndPerformFunction(sfd);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator ControlBottomCardResponse(CardController cardController)
        {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            List<Card> storedResultsCard = new List<Card>();

            IEnumerator coroutine;

            coroutine = this.GameController.RevealCards(cardController.TurnTakerController, this.TurnTaker.Deck, 1, storedResultsCard, true, RevealedCardDisplay.None, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            Card card = storedResultsCard.FirstOrDefault();
            if (card != null)
            {
                coroutine = this.GameController.SelectLocationAndMoveCard(this.DecisionMaker, card,
                    new List<MoveCardDestination>() {
                        new MoveCardDestination(this.TurnTaker.Deck, true),
                        new MoveCardDestination(this.TurnTaker.Trash, false)
                    },
                    cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.CleanupCardsAtLocations(new List<Location> { cardController.TurnTaker.Revealed }, this.TurnTaker.Deck, false, true, false, false, false, true, storedResultsCard);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyCardsUnderResponse(CardController cardController)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> dcaResults = new List<DestroyCardAction>();

            coroutine = this.GameController.SelectAndDestroyCards(cardController.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation, "cards under " + this.Card.Title), this.H - 1, false, this.H - 1, storedResultsAction: dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dcaResults.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Count() == this.H - 1)
            {
                coroutine = this.GameController.SelectAndDestroyCards(cardController.HeroTurnTakerController, new LinqCardCriteria((Card c) => (c.IsDiversion || c.IsOngoing)), 1, false, 1, storedResultsAction: dcaResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("2 cards were not destroyed, so no Diversion or Ongoing card will be destroyed.", Priority.Low, null);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}