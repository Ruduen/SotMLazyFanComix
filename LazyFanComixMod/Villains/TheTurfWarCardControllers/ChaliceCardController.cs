using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class ChaliceCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public ChaliceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddReduceDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card.IsCultist && dda.Target.IsCultist, (DealDamageAction dda) => 2);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyCardResponse, TriggerType.DestroyCard);
        }

        private IEnumerator DestroyCardResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> dca = new List<DestroyCardAction>();
            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => !this.IsVillain(c) && this.IsOngoing(c)), 1, storedResultsAction: dca, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dca != null && dca.Count() > 0)
            {
                IEnumerable<Card> destroyedCards = dca.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Select((DestroyCardAction dca) => dca.CardToDestroy.Card);
                coroutine = MoveCardsUnderFigureheadWithSharedKeyword(destroyedCards);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}