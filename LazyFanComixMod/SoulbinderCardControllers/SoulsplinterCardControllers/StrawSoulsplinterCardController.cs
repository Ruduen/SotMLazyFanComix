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
                this.GetPowerNumeral(0, 1),   // Damage.
                this.GetPowerNumeral(1, 2),   // Self Damage. 
                this.GetPowerNumeral(2, 1),   // Number of tokens
                this.GetPowerNumeral(3, 1)    // Number of Rituals
            };
            return numerals;
        }

        protected override IEnumerator UseUniquePower(List<int> powerNumerals)
        {
            
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            coroutine = this.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => !c.IsHero, powerNumerals[0], DamageType.Infernal, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


    }
}