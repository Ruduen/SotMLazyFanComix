using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class StrawSoulsplinterCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public StrawSoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // You may play a one-shot or a ritual.
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.IsOneShot || c.DoKeywordsContain("ritual"), "one-shot or ritual"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 1),   // Number of Targets
                            this.GetPowerNumeral(1, 2),   // Damage.
                            this.GetPowerNumeral(2, 2)    // HP to regain.
            };
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, numerals[1], DamageType.Toxic);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals 1 Target 2 Toxic Damage
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, targetList.FirstOrDefault()), numerals[1], DamageType.Toxic, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Regains 2 HP.
                coroutine = this.GameController.GainHP(targetList.FirstOrDefault(), numerals[2], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}