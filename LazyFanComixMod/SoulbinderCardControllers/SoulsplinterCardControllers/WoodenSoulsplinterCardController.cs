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

        protected override IEnumerator UseUniquePower()
        {
            List<int> powerNumerals = new List<int>(){
                            this.GetPowerNumeral(0, 1) // HP to regain
            };
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Each of your Hero Targets regains 1 HP.
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero && c.Owner == this.TurnTaker, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}