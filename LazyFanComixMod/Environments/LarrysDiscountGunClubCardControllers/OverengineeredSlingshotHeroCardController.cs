using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class OverengineeredSlingshotHeroCardController : CardController
    {
        public OverengineeredSlingshotHeroCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 4),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
            };

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.DecisionMaker.CharacterCard), null, numerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.DecisionMaker.CharacterCard), null, numerals[2], DamageType.Fire)
            }, null, null, 0, numerals[0], false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}