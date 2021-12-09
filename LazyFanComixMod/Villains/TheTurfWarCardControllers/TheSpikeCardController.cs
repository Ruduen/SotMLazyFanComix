using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheSpikeCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public TheSpikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartDamageResponse, TriggerType.DealDamage);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndDamageResponse, TriggerType.DealDamage);

        }

        private IEnumerator StartDamageResponse(PhaseChangeAction pca)
        {

            Func<DealDamageAction, int> EnvironmentCardsNotUnder = (DealDamageAction dda) => this.FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.IsEnvironment).Count();

            // Increase trigger. 
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, EnvironmentCardsNotUnder);

            IEnumerator coroutine = this.DealDamageToHighestHP(this.Card, 1, (Card c) => !c.IsThug, (Card c) => 3, DamageType.Melee, numberOfTargets: () => this.H - 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        private IEnumerator EndDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.DealDamageToHighestHP(this.Card, 1, (Card c) => !c.IsThug, (Card c) => 3, DamageType.Melee, numberOfTargets: () => this.H - 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}
