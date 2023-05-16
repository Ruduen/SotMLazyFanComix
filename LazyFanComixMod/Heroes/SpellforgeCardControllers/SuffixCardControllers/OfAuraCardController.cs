using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Spellforge
{
    public class OfAuraCardController : SpellforgeModifierSharedCardController
    {
        public OfAuraCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        private List<CardSource> __createdCardSources = new List<CardSource>();

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Destroy.
            coroutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => this.IsOngoing(c) || c.IsEnvironment, "ongoing or environment"), true, null, null, this.GetCardSource());
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
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda) && dda.DidDealDamage && dda.Target.IsHeroCharacterCard,
                RunResponse,
                new TriggerType[] { TriggerType.DealDamage },
                TriggerTiming.After);
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;

            // Draw Response - Should target only characters, but generic sanity check is included.
            if (dda?.Target?.Owner?.IsHero == true && dda?.Target?.IsHeroCharacterCard == true)
            {
                coroutine = this.GameController.DrawCard(dda.Target.Owner.ToHero(), cardSource: this._cardControllerActivatingModifiers.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}