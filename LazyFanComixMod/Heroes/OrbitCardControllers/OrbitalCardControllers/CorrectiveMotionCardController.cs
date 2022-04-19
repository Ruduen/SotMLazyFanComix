using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class CorrectiveMotionCardController : SharedOrbitalCardController
    {
        public CorrectiveMotionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddUniqueTriggers()
        {
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda?.DamageSource != null && !dda.DamageSource.IsSameCard(this.CharacterCard) && dda.DamageSource.Card.IsTarget && dda.Target == this.GetCardThisCardIsNextTo(), IncreaseAndDestroyResponse, new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.DestroySelf }, TriggerTiming.Before);
        }

        private IEnumerator IncreaseAndDestroyResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> yncds = new List<YesNoCardDecision>();

            coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.Custom, this.Card, dda, yncds, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yncds))
            {
                coroutine = this.GameController.IncreaseDamage(dda, 2, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Do you want to increase this damage by 2 and destroy Corrective Motion?", "Should they increase this damage by 2 and destroy Corrective Motion?", "Vote for increasing this damage by 2 and destroying Corrective Motion?", "increasing this damage by 2 and destroy Corrective Motion");
        }
    }
}