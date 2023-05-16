using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.TheTurfWar
{
    public class ZenithCardController : SharedMoveCardUnderSharedKeywordCardController
    {
        public ZenithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsEnvironmentCard && dda.Target.IsThug, (DealDamageAction dda) => 2);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, MoveCardResponse, TriggerType.MoveCard);
        }

        private IEnumerator MoveCardResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = MoveCardsUnderFigureheadWithSharedKeyword(this.FindEnvironment().TurnTaker.Deck.GetTopCards(1));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}