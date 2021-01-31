using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public class SupplyPackCardController : TrailblazerOnPlayPositionCardController
    {
        public SupplyPackCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType ResponseTriggerType()
        {
            return TriggerType.UsePower;
        }

        protected override IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            // All of the relevant ownership means the positions should only have 1 power, so we can skip more complex handling.

            if (cepa.CardEnteringPlay.HasPowers)
            {
                IEnumerator coroutine = this.GameController.UsePower(cepa.CardEnteringPlay, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}