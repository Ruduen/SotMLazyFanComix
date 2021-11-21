using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheLeechesCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public TheLeechesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyCardResponse, TriggerType.DestroyCard);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DealDamageResponse, TriggerType.DealDamage, null, false);

        }

        private IEnumerator DestroyCardResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> dca = new List<DestroyCardAction>();
            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => !this.IsVillain(c) && c.IsOngoing), 2, storedResultsAction: dca, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dca != null && dca.Count() > 0)
            {
                IEnumerable<Card> destroyedCards = dca.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Select((DestroyCardAction dca) => dca.CardToDestroy.Card);
                coroutine = MoveCardsUnderFigureheadWithSharedKeyword(destroyedCards);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, this.Card, (Card c) => true, 2, DamageType.Infernal, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}
