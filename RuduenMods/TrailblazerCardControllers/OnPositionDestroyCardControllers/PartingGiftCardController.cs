using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public class PartingGiftCardController : TrailblazerOnDestroyedPositionCardController
    {
        public PartingGiftCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ResponseAction(DestroyCardAction dca)
        {
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Fire, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override TriggerType[] ResponseTriggerTypes()
        {
            return new TriggerType[] { TriggerType.DealDamage };
        }
    }
}