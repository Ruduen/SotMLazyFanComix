using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Orbit
{
    public class HeavyBlockadeCardController : CardController
    {
        public HeavyBlockadeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.DealDamage);
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
        }

        public override IEnumerator UsePower(int index = 0)
        {

            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3)
            };

            // Deal <a> target <b> damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}