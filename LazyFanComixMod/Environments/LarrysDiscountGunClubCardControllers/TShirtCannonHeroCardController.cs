using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class TShirtCannonHeroCardController : SharedHeroGunEarnedCardController
    {
        public TShirtCannonHeroCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3),
                this.GetPowerNumeral(2, 2),
                this.GetPowerNumeral(3, 2),
            };
            List<DiscardCardAction> dcas = new List<DiscardCardAction>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndDiscardCard(this.DecisionMaker, storedResults: dcas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Trigger to increase damage dealt.
            List<ITrigger> tempIncrease = new List<ITrigger>() {
                this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this && dcas.Any((DiscardCardAction dca) => dca.WasCardDiscarded && this.IsEquipment(dca.CardToDiscard)), (DealDamageAction dda) => numerals[2]) ,
                this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this && dcas.Any((DiscardCardAction dca) => dca.WasCardDiscarded && dca.CardToDiscard.DoKeywordsContain("religalia")), (DealDamageAction dda) => numerals[3])
            };

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.DecisionMaker.CharacterCard), numerals[1], DamageType.Projectile, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach(ITrigger t in tempIncrease)
            {
                this.RemoveTrigger(t);
            }

        }


    }
}