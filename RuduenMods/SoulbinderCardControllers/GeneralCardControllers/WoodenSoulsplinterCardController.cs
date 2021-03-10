using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class WoodenSoulsplinterCardController : CardController
    {
        public WoodenSoulsplinterCardController(Card card, TurnTakerController turnTakerController)
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
            List<int> powerNumerals = new List<int>(){
                            this.GetPowerNumeral(0, 1), // HP to regain
                            this.GetPowerNumeral(1, 1)  // Damage to inflict.
            };
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Each of your Hero Targets regains 2 HP.
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero && c.Owner == this.TurnTaker, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}