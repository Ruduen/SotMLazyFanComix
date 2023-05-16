using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Spellforge
{
    public class OfHealingCardController : SpellforgeModifierSharedCardController
    {
        public OfHealingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Heal.
            coroutine = this.GameController.GainHP(this.CharacterCard, 3, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw 3.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override ITrigger AddModifierTriggerOverride()
        {
            return this.AddTrigger<DealDamageAction>(
                // Criteria: Core checks, target is non hero, damage source is not the specific effect causing recursion (which is via activating modifiers).
                // Note: This is not perfect, but due to card source matching needing to be 'sustained' rather than tracked via the source card each time, it's as close as I can get.
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda) && dda.DidDealDamage && dda.Target.IsHero,
                RunResponse,
                new TriggerType[] { TriggerType.DealDamage },
                TriggerTiming.After);
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine = this.GameController.GainHP(dda.Target, 3, cardSource: this._cardControllerActivatingModifiers.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}