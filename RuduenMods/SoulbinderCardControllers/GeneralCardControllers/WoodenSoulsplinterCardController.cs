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


        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>(){
                            this.GetPowerNumeral(0, 2) // HP to regain
            };
            List<Card> targetList = new List<Card>();
            IEnumerator coroutine;

            // Each of your Hero Targets regains 2 HP.
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero && c.Owner == this.TurnTaker, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}