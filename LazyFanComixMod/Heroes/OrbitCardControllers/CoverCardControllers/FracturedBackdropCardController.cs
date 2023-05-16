using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

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
            this.AddTrigger<DealDamageAction>(
                (DealDamageAction dda) => dda.DidDealDamage && dda.Target == this.Card && dda.DamageSource?.Card == this.CharacterCard,
                OnDamageResponse, TriggerType.DealDamage, TriggerTiming.After
                );
        }

        private IEnumerator OnDamageResponse(DealDamageAction dda)
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Projectile, 4, false, 0, cardSource: this.GetCardSource());
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 1, DamageType.Projectile, 3, false, 0, cardSource: this.GetCardSource());
        }
    }
}