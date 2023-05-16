using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

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
            this.AddWhenDestroyedTrigger(OnDestroyResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage });
            this.AddTrigger<DealDamageAction>(
                (DealDamageAction dda) => dda.DidDealDamage && dda.Target == this.Card && dda.DamageSource?.Card == this.CharacterCard,
                OnDamageResponse, TriggerType.DealDamage, TriggerTiming.After
                );
        }

        private IEnumerator OnDamageResponse(DealDamageAction dda)
        {
            //List<DealDamageAction> damageInstances = new List<DealDamageAction>() {
            //    new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 1, DamageType.Projectile),
            //    new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController,this.Card), null, 1, DamageType.Toxic)
            //};

            //return this.SelectTargetsAndDealMultipleInstancesOfDamage(damageInstances, minNumberOfTargets: 1, maxNumberOfTargets: 1);

            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Toxic, 2, false, 0, cardSource: this.GetCardSource());
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsOngoing(c) || c.IsEnvironment, "ongoing or environment"), 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 1, DamageType.Toxic, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}