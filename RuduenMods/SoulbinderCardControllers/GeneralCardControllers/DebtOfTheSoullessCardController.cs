using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Soulbinder
{
    public class DebtOfTheSoullessCardController : SoulbinderSharedYourTargetDamageCardController
    {
        private readonly string _propertyKey = "SoulbinderFirstTimeDamage";
        private ITrigger _reduceTrigger;

        public DebtOfTheSoullessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_propertyKey, "{0} can no longer reduce damage this turn.", "{0} has not yet reduced and redirected damage this turn.", null);
        }

        public override void AddTriggers()
        {
            _reduceTrigger = this.AddTrigger<DealDamageAction>((DealDamageAction dd) => !this.IsPropertyTrue(_propertyKey) && dd.Amount > 0 && (dd.DamageSource.Card != null && dd.DamageSource.Card.IsHero && dd.Target != null && dd.Target.Owner == this.TurnTaker), ReduceResponse, new TriggerType[] { TriggerType.ReduceDamage, TriggerType.DrawCard }, TriggerTiming.Before, orderMatters: true);
            this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_propertyKey), TriggerType.Hidden);
        }

        private IEnumerator ReduceResponse(DealDamageAction dd)
        {
            IEnumerator coroutine;

            this.SetCardPropertyToTrueIfRealAction(_propertyKey);

            coroutine = this.GameController.ReduceDamage(dd, 1, this._reduceTrigger, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}