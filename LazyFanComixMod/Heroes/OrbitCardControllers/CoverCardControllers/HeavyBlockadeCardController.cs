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
            this.AddTrigger<DealDamageAction>(
                (DealDamageAction dda) => dda.DidDealDamage && dda.Target == this.Card && dda.DamageSource?.Card == this.CharacterCard,
                OnDamageResponse, TriggerType.DealDamage, TriggerTiming.After
                );
        }

        private IEnumerator OnDamageResponse(DealDamageAction dda)
        {
            List<DealDamageAction> damageInstances = new List<DealDamageAction>() {
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 2, DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 1, DamageType.Melee)
            };

            return this.SelectTargetsAndDealMultipleInstancesOfDamage(damageInstances, minNumberOfTargets: 1, maxNumberOfTargets: 1);
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
        }
    }
}