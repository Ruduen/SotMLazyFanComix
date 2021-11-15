using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Trailblazer
{
    // TODO: TEST!
    public class StrikingZoneCardController : TrailblazerPositionCardController
    {
        public StrikingZoneCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Increase the first damage by Trailblazer each turn by 2.
            this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.CharacterCard), 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1)
            };

            // Deal each non-Hero target 1 damage.
            coroutine = this.GameController.DealDamage(this.HeroTurnTakerController, this.HeroTurnTaker.CharacterCard, new Func<Card, bool>((Card c) => !c.IsHero), powerNumerals[0], DamageType.Projectile, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}