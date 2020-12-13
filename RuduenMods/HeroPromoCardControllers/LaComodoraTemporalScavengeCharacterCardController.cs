using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.LaComodora
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
            WhenCardIsDestroyedStatusEffect whenCardIsDestroyedStatusEffect = new WhenCardIsDestroyedStatusEffect(this.CardWithoutReplacements, "WhenEquipIsDestroyedFlip", "Whenever one of " + turnTakerName +"'s Equipment would be destroyed, they may flip it face-down instead..", new TriggerType[]
            { TriggerType.FlipCard }, this.HeroTurnTaker, this.Card, null);
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "equipment" };
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.OwnedBy = this.HeroTurnTaker;
            whenCardIsDestroyedStatusEffect.UntilEndOfNextTurn(this.HeroTurnTaker);
            whenCardIsDestroyedStatusEffect.Priority = new StatusEffectPriority?(StatusEffectPriority.High);
            whenCardIsDestroyedStatusEffect.CanEffectStack = false;
            whenCardIsDestroyedStatusEffect.PostDestroyDestinationMustBeChangeable = true;
            coroutine = this.AddStatusEffect(whenCardIsDestroyedStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

#pragma warning disable IDE0060 // Remove unused parameter

        public IEnumerator WhenEquipIsDestroyedFlip(DestroyCardAction destroy, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            IEnumerator coroutine;
            if (hero != null && destroy.CanBeCancelled)
            {
                if (destroy.CardToDestroy.Card.IsMissionCard)
                {
                    coroutine = this.GameController.SendMessageAction("Mission cards cannot be flipped face-down, so they will still be destroyed.", Priority.Low, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
                    coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.PreventDestruction, destroy.CardToDestroy.Card, storedResults: storedResults, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    if (this.DidPlayerAnswerYes(storedResults))
                    {
                        destroy.PreventMoveToTrash = true;

                        // Cancel the destruction.
                        coroutine = this.CancelAction(destroy, false, true, null, true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                        //Flip card. Original implementation, but UI made it weird due to destruction interception. 
                        //coroutine = this.GameController.FlipCard(destroy.CardToDestroy, cardSource: this.GetCardSource());

                        //Move the card to in-play while flipping it, since it's currently in an ambiguous zone visually-speaking. (The destruction made it vanish.) 
                        coroutine = this.GameController.MoveCard(this.DecisionMaker, destroy.CardToDestroy.Card, this.HeroTurnTaker.PlayArea, playCardIfMovingToPlayArea: false, flipFaceDown: true, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }
            }
        }

#pragma warning restore IDE0060 // Remove unused parameter
    }
}