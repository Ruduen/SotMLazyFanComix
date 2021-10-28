using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public class StrawSoulsplinterCardController : SoulbinderSharedSoulsplinterCardController
    {
        public StrawSoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override List<int> GetUniquePowerNumerals()
        {
            List<int> numerals = new List<int>(){
                this.GetPowerNumeral(0, 1),   // Number of Targets
                this.GetPowerNumeral(1, 2),   // Damage.
                this.GetPowerNumeral(2, 3),   // HP to regain.
                this.GetPowerNumeral(3, 1),   // Number of tokens
                this.GetPowerNumeral(4, 1)    // Number of Rituals
            };
            return numerals;
        }

        protected override IEnumerator UseUniquePower(List<int> powerNumerals)
        {
            
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, powerNumerals[1], DamageType.Toxic);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals 1 Target 2 Toxic Damage
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, targetList.FirstOrDefault()), powerNumerals[1], DamageType.Toxic, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Regains 3 HP.
                coroutine = this.GameController.GainHP(targetList.FirstOrDefault(), powerNumerals[2], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }


    }
}