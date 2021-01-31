using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Trailblazer
{
    // TODO: TEST!
    public class DefensiveBulwarkCardController : TrailblazerPositionCardController
    {

        private const string _damageProperty = "FirstTimeTakingDamage";
        private ITrigger _reduceDamageTrigger;
        public DefensiveBulwarkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_damageProperty, "{0} can no longer reduce damage this turn.", "{0} has not yet reduced damage this turn.");
        }

        public override void AddTriggers()
        {
            // Reduce the first damage to Trailblazer each turn by 2. 
            this._reduceDamageTrigger = this.AddReduceDamageTrigger((DealDamageAction dd) => !this.IsPropertyTrue(_damageProperty, null), new Func<DealDamageAction, IEnumerator>(this.ReduceDamageResponse), (Card c) => c == this.CharacterCard, true);
            this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_damageProperty), TriggerType.Hidden);
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction dd)
        {
            this.SetCardPropertyToTrueIfRealAction(_damageProperty);
            IEnumerator coroutine = this.GameController.ReduceDamage(dd, 2, this._reduceDamageTrigger, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 2)
            };

            // Each Hero Target regains 2 HP.  
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}