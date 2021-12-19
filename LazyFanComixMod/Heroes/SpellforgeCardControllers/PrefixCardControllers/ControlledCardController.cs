using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Spellforge
{
    public class ControlledCardController : SpellforgeModifierSharedCardController
    {
        public ControlledCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        private ITrigger __reduceDamageTrigger = null;

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            // Select a hero to play a card.
            coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override ITrigger AddModifierTriggerOverride()
        {
            __reduceDamageTrigger = this.AddTrigger<DealDamageAction>(
                // Criteria: Core and either more than 1 to hero, or non-hero. 
                (DealDamageAction dda) => CoreDealDamageActionCriteria(dda) && (!dda.Target.IsHero || (dda.Amount > 1 && dda.Target.IsHero)),
               RunResponse,
                new TriggerType[] { TriggerType.ReduceDamage, TriggerType.IncreaseDamage }, TriggerTiming.Before, orderMatters: true);

            return __reduceDamageTrigger;
        }

        protected IEnumerator RunResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;

            if (dda.Target.IsHero)
            {
                // If this is the hero damage, reduce the damage to 1. 
                coroutine = this.GameController.ReduceDamage(dda, dda.Amount - 1, __reduceDamageTrigger, cardSource: this._cardControllerActivatingModifiers.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // If this is non-hero damage, increase the damage by 1. 
                coroutine = this.GameController.IncreaseDamage(dda, (dda) => 1, this._cardControllerActivatingModifiers.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected override void ClearOtherValues()
        {
            __reduceDamageTrigger = null;
            return;
        }
    }
}