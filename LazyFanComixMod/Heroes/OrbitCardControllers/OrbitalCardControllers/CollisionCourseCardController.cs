using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class CollisionCourseCardController : SharedOrbitalCardController
    {
        public CollisionCourseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }


        public override void AddUniqueTriggers()
        {
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card != this.CharacterCard && dda.DamageSource.Card.IsTarget && dda.Target == this.GetCardThisCardIsNextTo(), DamageAndDestroyResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
        }

        private IEnumerator DamageAndDestroyResponse(DealDamageAction ddaa)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scds = new List<SelectCardDecision>();

            Card nextTo = this.GetCardThisCardIsNextTo();
            if (nextTo == null) { yield break; }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, nextTo), 3, DamageType.Melee, 1, false, 0, additionalCriteria: (Card c) => c != nextTo, storedResultsDecisions: scds, selectTargetsEvenIfCannotDealDamage: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scds.Any((SelectCardDecision scd) => scd.SelectedCard != null))
            {
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }


    }
}