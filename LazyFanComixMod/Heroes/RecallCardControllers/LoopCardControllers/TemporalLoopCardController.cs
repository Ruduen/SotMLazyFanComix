using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class TemporalLoopCardController : RecallSharedDamageSelfCardController
    {
        public TemporalLoopCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => GoToStartOfTurn(),
                new TriggerType[] { TriggerType.PhaseChange, TriggerType.SkipPhase, TriggerType.MoveCard, TriggerType.DealDamage });
        }

        protected IEnumerator GoToStartOfTurn()
        {
            TurnPhase oldTurnPhase = this.Game.ActiveTurnPhase;
            TurnPhase destinationTurnPhase = (from tp in this.TurnTaker.TurnPhases
                                              where tp.IsStart || tp.IsBeforeStart
                                              select tp).FirstOrDefault();
            IEnumerator coroutine;

            coroutine = this.DamageSelfMoveCard();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SkipToTurnPhase(destinationTurnPhase, false, true, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Temporarily prevent all other cards/turns/etc. from acting unexpectedly.
            this.AddInhibitorException((GameAction ga) => true);
            this.GameController.AddTemporaryTriggerInhibitor<PhaseChangeAction>((ITrigger t) => ((t is PhaseChangeTrigger && !(t as PhaseChangeTrigger).PhaseCriteria(destinationTurnPhase.Phase) && !(t as PhaseChangeTrigger).TurnTakerCriteria(destinationTurnPhase.TurnTaker)) || (!(t is PhaseChangeTrigger) && !t.Types.Contains(TriggerType.ChangePostDestroyDestination))) && t.CardSource != null && t.CardSource.Card != this.Card, (PhaseChangeAction p) => p.FromPhase == oldTurnPhase, this.GetCardSource());

            // Move all one-shots to the trash to prevent odd 'stasis' condition.
            coroutine = this.GameController.MoveCards(this.TurnTakerController,
                this.FindCardsWhere((Card c) => c.IsOneShot && c.IsInPlayAndHasGameText),
                (Card c) => this.FindCardController(c).GetTrashDestination(),
                cardSource: this.GetCardSource()
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Inhibit any remaining actions except phase change actions from the old turn or play card actions which are explicitly canceled.
            IEnumerable<GameAction> actions = (from ga in this.GameController.UnresolvedActions where ga.CardSource == null || ga.CardSource.Card != this.Card select ga);
            foreach (GameAction action in actions)
            {
                if (action.CardSource != null)
                {
                    this.GameController.AddTemporaryInhibitor<GameAction>(action.CardSource.CardController, (GameAction ga) => this.GameController.UnresolvedActions.Count() == 0 || (ga is PhaseChangeAction && (ga as PhaseChangeAction).FromPhase == oldTurnPhase), this.GetCardSource(), false);
                }
                else if (action is PlayCardAction)
                {
                    coroutine = this.CancelAction(action, false);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}