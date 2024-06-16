using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Thri
{
    public class CallTheShotCardController : CardController
    {
        public CallTheShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0,1),
                this.GetPowerNumeral(1,1),
                this.GetPowerNumeral(2,1)
            };
            IEnumerator coroutine;


            coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, powerNumerals[1], false, powerNumerals[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Another hero may use a power. 
            coroutine = this.GameController.SelectHeroToUsePower(this.DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}