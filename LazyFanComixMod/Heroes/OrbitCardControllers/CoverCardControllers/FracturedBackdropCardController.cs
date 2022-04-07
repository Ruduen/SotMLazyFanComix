using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Orbit
{
    public class FracturedBackdropCardController : CardController
    {
        public FracturedBackdropCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.DealDamage);
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            List<DealDamageAction> damageInstances = new List<DealDamageAction>() {
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 1, DamageType.Melee),
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 1, DamageType.Projectile),
            };

            return this.DealMultipleInstancesOfDamage(damageInstances, (Card c) => !c.IsHero);
        }

        public override IEnumerator UsePower(int index = 0)
        {

            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 4),
                this.GetPowerNumeral(1, 1)
            };

            // Deal up to <a> target <b> damage.
            return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, 0, cardSource: this.GetCardSource());
        }

    }
}