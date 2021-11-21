using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public abstract class TheTurfWarSharedCharacterCardController : VillainCharacterCardController
    {
        public override bool CanBeDestroyed
        {
            get { return false; }
        }
        public TheTurfWarSharedCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            // TODO: Verify flipped side trigger. (Non-Flipped Triggers are Unique.) 
            if (!this.Card.IsFlipped)
            {
                this.AddSideTrigger(this.AddUniqueActiveTrigger());
            }
            else
            {
                this.AddSideTrigger(this.AddUniqueIncapacitatedTrigger());
                this.AddSideTrigger(this.AddCannotDealDamageTrigger((Card c) => c == base.Card));
            }
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            CardSource cardSource = flip.CardSource;
            if (cardSource == null && flip.ActionSource != null)
            {
                cardSource = flip.ActionSource.CardSource;
            }
            if (cardSource == null)
            {
                cardSource = this.GetCardSource();
            }

            if (!this.Card.IsFlipped)
            {
                IEnumerator coroutine = this.GameController.RemoveTarget(this.Card, true, cardSource);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                IEnumerator coroutine = this.GameController.MakeTargettable(this.Card, 70, 15, cardSource);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected abstract ITrigger AddUniqueIncapacitatedTrigger();

        protected abstract ITrigger AddUniqueActiveTrigger();

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            FlipCardAction action = new FlipCardAction(this.GameController, this, false, false, destroyCard.ActionSource);
            IEnumerator coroutine = this.DoAction(action);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}
