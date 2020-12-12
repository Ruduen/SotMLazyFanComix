using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace RuduenWorkshop.BreachMage
{
    public class ZapCardController : BreachMageSharedSpellCardController
    {
        public ZapCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy == this && !this.GameController.IsCardIndestructible(this.Card), this.MoveInsteadResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.Before);
        }

        protected override IEnumerator ActivateCast()
        {
            IEnumerator coroutine;
            // Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Lightning, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public IEnumerator MoveInsteadResponse(DestroyCardAction d)
        {
            IEnumerator coroutine;

            // Cancel destruction. 
            coroutine = this.CancelAction(d);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Move into hand instead. 
            coroutine = this.GameController.MoveCard(this.DecisionMaker, this.Card, this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}