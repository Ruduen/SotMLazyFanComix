using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Soulbinder
{
    public class ClaySoulsplinterCardController : SoulbinderSharedSoulsplinterCardController
    {
        public ClaySoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override List<int> GetUniquePowerNumerals()
        {
            List<int> numerals = new List<int>(){
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };
            return numerals;
        }

        protected override IEnumerator UseUniquePower(List<int> powerNumerals)
        {
            IEnumerator coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}