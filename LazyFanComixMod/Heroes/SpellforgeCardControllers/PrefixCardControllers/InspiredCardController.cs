using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Spellforge
{
    public class InspiredCardController : SpellforgeModifierSharedCardController
    {
        public InspiredCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = this.EachPlayerDrawsACard();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override ITrigger AddModifierTriggerOverride()
        {
            return this.AddTrigger<DealDamageAction>(
                // Criteria: Core. Needs to be done this way so card sources are correct.
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda),
               RunResponse,
                new TriggerType[] { TriggerType.IncreaseDamage },
                TriggerTiming.Before);
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine = this.GameController.IncreaseDamage(dda, (dda) => 2, this._cardControllerActivatingModifiers.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}