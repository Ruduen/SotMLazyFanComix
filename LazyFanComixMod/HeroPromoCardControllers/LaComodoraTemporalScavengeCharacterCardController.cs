using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.LaComodora
{
    public class LaComodoraTemporalScavengeCharacterCardController : PromoDefaultCharacterCardController
    {
        public LaComodoraTemporalScavengeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            string turnTakerName;
            IEnumerator coroutine;

            if (this.TurnTaker.IsHero) { turnTakerName = this.TurnTaker.Name; }
            else { turnTakerName = this.Card.Title; }

            // Flip a card face-up.
            coroutine = this.GameController.SelectAndFlipCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.HeroTurnTaker.PlayArea && c.IsFaceDownNonCharacter && !c.IsMissionCard, "face-down cards in " + turnTakerName + "'s play area"), 1, false, false, 1, null, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Set up an effect to respond when your equipment is destroyed
            WhenCardIsDestroyedStatusEffect whenCardIsDestroyedStatusEffect = new WhenCardIsDestroyedStatusEffect(this.CardWithoutReplacements, "WhenEquipIsDestroyed", "Whenever one of " + turnTakerName + "'s Equipment would be destroyed, they may put it in their play area face-down.", new TriggerType[]
            { TriggerType.FlipCard }, this.HeroTurnTaker, this.Card, null);
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "equipment" };
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.OwnedBy = this.HeroTurnTaker;
            whenCardIsDestroyedStatusEffect.UntilEndOfNextTurn(this.HeroTurnTaker);
            whenCardIsDestroyedStatusEffect.Priority = new StatusEffectPriority?(StatusEffectPriority.High);
            whenCardIsDestroyedStatusEffect.CanEffectStack = false;
            coroutine = this.AddStatusEffect(whenCardIsDestroyedStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

#pragma warning disable IDE0060 // Remove unused parameter

        public IEnumerator WhenEquipIsDestroyed(DestroyCardAction destroy, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            IEnumerator coroutine;
            if (hero != null && destroy.CanBeCancelled)
            {
                if (Card.IsMissionCard)
                {
                    coroutine = this.GameController.SendMessageAction("Mission cards cannot be flipped face-down, so "+ this.CharacterCard.Title +" cannot flip it instead of destroying it.", Priority.Low, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
                    coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.Custom, destroy.CardToDestroy.Card, storedResults: storedResults, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    if (this.DidPlayerAnswerYes(storedResults))
                    {
                        // Needs to be done after because most 'after destruction' effects are actually 'when destroyed' effects, and we can't move the card back before it's destroyed without causing more problems than it's worth.
                        destroy.SetPostDestroyDestination(this.TurnTaker.PlayArea);
                        destroy.PostDestroyDestinationCanBeChanged = false;
                        destroy.AddAfterDestroyedAction(() => this.FlipResponse(hero, destroy.CardToDestroy.Card), this);
                    }
                }
            }
        }

#pragma warning restore IDE0060 // Remove unused parameter

        public IEnumerator FlipResponse(HeroTurnTaker hero, Card card)
        {
            HeroTurnTakerController decisionMaker = this.FindHeroTurnTakerController(hero);
            IEnumerator coroutine;

            if (card.IsMissionCard)
            {
                coroutine = this.GameController.SendMessageAction("Mission cards cannot be flipped face-down. This should've already been stopped. How did you even get here?", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.FlipCard(this.GameController.FindCardController(card), cardSource: this.GetCardSource(), allowBackToFront: false);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Move the destroyed card to your play area face-down?", "Move the destroyed card to the play area face-down?", "Move the destroyed card to the play area face-down?", "Move the destroyed card to the play area face-down?");
        }
    }
}