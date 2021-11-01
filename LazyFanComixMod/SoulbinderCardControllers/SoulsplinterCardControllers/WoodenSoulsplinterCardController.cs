using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Soulbinder
{
    public class WoodenSoulsplinterCardController : SoulbinderSharedSoulsplinterCardController
    {
        public WoodenSoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override List<int> GetUniquePowerNumerals()
        {
            List<int> numerals = new List<int>(){
                this.GetPowerNumeral(0, 1),   // HP to Regain
                this.GetPowerNumeral(1, 2),   // Amount of damage
                this.GetPowerNumeral(2, 1),   // Number of tokens
                this.GetPowerNumeral(3, 1)    // Number of Rituals
            };
            return numerals;
        }

        protected override IEnumerator UseUniquePower(List<int> powerNumerals)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}