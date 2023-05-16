using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Orbit
{
    public class LandscapeAwarenessCardController : CardController
    {
        public LandscapeAwarenessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource?.Card == this.CharacterCard && dd.Target.IsHero);
            this.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource?.Card == this.CharacterCard && dd.Target.IsHero,
                (DealDamageAction dd) => this.GameController.MakeDamageUnincreasable(dd, base.GetCardSource()), TriggerType.MakeDamageUnincreasable, TriggerTiming.Before);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, new LinqCardCriteria((Card c) => c.IsCover, "cover"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal <a> target <b> damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}