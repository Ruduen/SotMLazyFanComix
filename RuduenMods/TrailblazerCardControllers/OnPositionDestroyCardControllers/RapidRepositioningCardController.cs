using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public class RapidRepositioningCardController : TrailblazerOnDestroyedPositionCardController
    {
        public RapidRepositioningCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ResponseAction(DestroyCardAction dca)
        {
            IEnumerator coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override TriggerType[] ResponseTriggerTypes()
        {
            return new TriggerType[] { TriggerType.PlayCard };
        }
    }
}