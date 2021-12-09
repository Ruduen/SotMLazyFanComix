using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheBrutesCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public TheBrutesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, MoveCardResponse, TriggerType.MoveCard);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DealDamageResponse, TriggerType.DealDamage, null, false);

        }

        private IEnumerator MoveCardResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = MoveCardsUnderFigureheadWithSharedKeyword(this.FindEnvironment().TurnTaker.Deck.GetTopCards(2));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.DealDamageToHighestHP(this.Card, 1, (Card c) => !c.IsThug, (Card c) => 4, DamageType.Melee, numberOfTargets: () => this.H - 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}
