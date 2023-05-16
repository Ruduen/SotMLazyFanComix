using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Spellforge
{
    public class OfDisruptionCardController : SpellforgeModifierSharedCardController
    {
        public OfDisruptionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        private List<CardSource> __createdCardSources = new List<CardSource>();

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Deal 1 target 2 sonic.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Sonic, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw 3.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override ITrigger AddModifierTriggerOverride()
        {
            __createdCardSources.Clear();
            return this.AddTrigger<DealDamageAction>(
                // Criteria: Core checks, target is non hero, damage source is not the specific effect causing recursion (which is via activating modifiers).
                // Note: This is not perfect, but due to card source matching needing to be 'sustained' rather than tracked via the source card each time, it's as close as I can get.
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda) && dda.DidDealDamage && !dda.Target.IsHero && !__createdCardSources.Contains(dda.CardSource),
                RunResponse,
                new TriggerType[] { TriggerType.DealDamage },
                TriggerTiming.After);
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            CardSource cs = this._cardControllerActivatingModifiers.GetCardSource();
            __createdCardSources.Add(cs);

            // Self Damage Response
            coroutine = this.DealDamage(dda.Target, dda.Target, 1, DamageType.Sonic, cardSource: cs);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override void ClearOtherValues()
        {
            __createdCardSources.Clear();
            return;
        }
    }
}