﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Soulbinder
{
    public class ClaySoulsplinterCardController : SoulbinderSharedSoulsplinterCardController
    {
        public ClaySoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator UseUniquePower()
        {
            // Select target to deal damage to.
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 1)
            };

            IEnumerator coroutine = this.GameController.DrawCards(this.DecisionMaker, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}