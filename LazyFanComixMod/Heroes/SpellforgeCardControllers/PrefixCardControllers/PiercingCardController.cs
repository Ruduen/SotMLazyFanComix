using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Spellforge
{
    public class PiercingCardController : SpellforgeModifierSharedCardController
    {
        public PiercingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Power.
            coroutine = this.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override ITrigger AddModifierTriggerOverride()
        {
            return this.AddTrigger<DealDamageAction>(
                // Criteria: Core.
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda),
               RunResponse,
                new TriggerType[] { TriggerType.MakeDamageIrreducible, TriggerType.MakeDamageNotRedirectable },
                TriggerTiming.Before);
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;

            // Deal damage response.
            coroutine = this.GameController.MakeDamageIrreducible(dda, this._cardControllerActivatingModifiers.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.MakeDamageNotRedirectable(dda, this._cardControllerActivatingModifiers.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}