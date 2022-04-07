using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Orbit
{
    public class VolatileBarricadeCardController : CardController
    {
        public VolatileBarricadeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddWhenDestroyedTrigger(OnDestroyResponse,new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage });
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Toxic, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }

        public override IEnumerator UsePower(int index = 0)
        {

            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 2)
            };

            // Deal <a> target <b> damage.
            return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Toxic, powerNumerals[0], false, 0, cardSource: this.GetCardSource());
        }

    }
}