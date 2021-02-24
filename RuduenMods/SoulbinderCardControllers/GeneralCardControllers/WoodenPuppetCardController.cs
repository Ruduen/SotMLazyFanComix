using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class WoodenPuppetCardController : SoulbinderSharedYourTargetDamageCardController
    {

        public WoodenPuppetCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // You may draw a card or play a card.
            coroutine = this.DrawACardOrPlayACard(this.DecisionMaker, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>(){
                            this.GetPowerNumeral(0, 1), // HP to regain
                            this.GetPowerNumeral(1, 1)  // Damage to inflict.
            };
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Each Hero Target regains 1 HP.  
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, powerNumerals[1], DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals 1 each non-Hero 1 Infernal Damage
                coroutine = this.GameController.DealDamage(this.DecisionMaker, targetList.FirstOrDefault(), (Card c) => !c.IsHero, powerNumerals[1], DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            }
        }
    }
}