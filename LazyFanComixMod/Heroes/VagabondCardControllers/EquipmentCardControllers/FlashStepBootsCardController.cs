using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Vagabond
{
    public class FlashStepBootsCardController : SharedEquipmentCardController
    {
        public FlashStepBootsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNum = this.GetPowerNumeral(0, 1);

            IEnumerator coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, powerNum, false, powerNum, new LinqCardCriteria((Card c) => c.IsOneShot, "one-shot"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}