using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheConduitCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public TheConduitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartDamageResponse, TriggerType.DealDamage);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndDamageResponse, TriggerType.DealDamage);

        }

        private IEnumerator StartDamageResponse(PhaseChangeAction pca)
        {

            Func<DealDamageAction, int> PlayersWithOngoings = (DealDamageAction dda) => this.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && tt.HasCardsWhere((Card c) => c.IsOngoing && c.IsInPlayAndHasGameText)).Count();

            // Increase trigger. 
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, PlayersWithOngoings);

            IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, this.Card, (Card c) => true, 1, DamageType.Infernal, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        private IEnumerator EndDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, this.Card, (Card c) => true, 1, DamageType.Infernal, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}
