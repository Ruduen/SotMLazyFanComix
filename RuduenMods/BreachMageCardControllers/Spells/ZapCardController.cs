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
            this.AddWhenDestroyedTrigger(this.MoveInsteadResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.ChangePostDestroyDestination });
        }

        protected override IEnumerator ActivateCast()
        {
            IEnumerator coroutine;
            // Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Lightning, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public IEnumerator MoveInsteadResponse(DestroyCardAction d)
        {
            CardSource actionCardSource = this.GetCardSource();

            if (d.PostDestroyDestinationCanBeChanged)
            {
                this.AddInhibitorException((GameAction ga) => (ga is MoveCardAction && (ga as MoveCardAction).CardSource == actionCardSource) || (ga is MessageAction && (ga as MessageAction).CardSource == actionCardSource));
                // Move into hand instead.
                d.SetPostDestroyDestination(this.HeroTurnTaker.Hand, showMessage: true, cardSource: actionCardSource);
                d.PostDestroyDestinationCanBeChanged = false;
                // Cannot remove inhibitor exception - the actual movement occurs afterwards as opposed to during an explicit coroutine. This should still be okay, functionally speaking - other areas also avoid removing inhibitor exceptions.
            }
            yield break;
        }
    }
}