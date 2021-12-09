using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class GlitterGunHeroCardController : CardController
    {
        public GlitterGunHeroCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 5),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };
            IEnumerator coroutine;


            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.DecisionMaker.CharacterCard), numerals[1], DamageType.Radiant, numerals[0], false, 0, addStatusEffect: (DealDamageAction dda) => this.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, numerals[2]), selectTargetsEvenIfCannotDealDamage: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


        }

    }
}