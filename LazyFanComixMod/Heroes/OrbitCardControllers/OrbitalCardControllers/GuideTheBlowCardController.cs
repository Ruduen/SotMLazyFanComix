using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class GuideTheBlowCardController : SharedOrbitalCardController
    {
        public GuideTheBlowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card == this.GetCardThisCardIsNextTo(), RedirectAndDestroyResponse, new TriggerType[] { TriggerType.RedirectDamage, TriggerType.DestroySelf }, TriggerTiming.Before);
        }

        private IEnumerator RedirectAndDestroyResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scds = new List<SelectCardDecision>();

            coroutine = this.GameController.SelectTargetAndRedirectDamage(this.DecisionMaker, null, dda, true, scds, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scds.Any((SelectCardDecision scd) => scd.Completed && scd.SelectedCard != null))
            {
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }


    }
}