using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ProbingStrikeCardController : SharedSoloCardController
    {
        public ProbingStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.DrawCards(this.DecisionMaker, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine =  this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardSource: this.GetCardSource());
        }

        protected override string IfNotSoloMessage()
        {
            return "no card can be played";
        }
    }
}