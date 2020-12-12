using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachIICardController : BreachMageSharedBreachController
    {
        public BreachIICardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseOpenPower()
        {
            // Play spell.
            IEnumerator coroutine = this.SelectAndPlayCardFromHand(this.DecisionMaker, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // You may use an additional power this turn.
            coroutine = this.AdditionalPhaseActionThisTurn(this.HeroTurnTaker, Phase.UsePower, 1, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}