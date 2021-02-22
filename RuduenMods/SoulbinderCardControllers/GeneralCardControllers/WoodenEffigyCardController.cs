using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class WoodenEffigyCardController : SoulbinderSharedYourTargetDamageCardController
    {

        public WoodenEffigyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // You may draw a card.
            coroutine = this.GameController.DrawCard(this.HeroTurnTaker, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // You may play a card.
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 1),   // Number of Targets
                            this.GetPowerNumeral(1, 2),   // Damage. 
                            this.GetPowerNumeral(2, 2)    // HP to regain.
            };
            List<Card> target = new List<Card>();
            IEnumerator coroutine;

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(target, numerals[1], DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if(target != null)
            {
                // That target deals 1 Target 1 Toxic Damage
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, target.FirstOrDefault()), numerals[1], DamageType.Toxic, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Regains 2 HP. 
                coroutine = this.GameController.GainHP(target.FirstOrDefault(), numerals[2], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}