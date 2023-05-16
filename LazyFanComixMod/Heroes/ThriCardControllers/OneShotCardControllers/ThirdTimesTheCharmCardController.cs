using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class ThirdTimesTheCharmCardController : CardController
    {
        public ThirdTimesTheCharmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Draw 3 cards.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Use the innate power 3 times, overriding normal restrictions.
            coroutine = this.GameController.SelectAndUsePower(this.DecisionMaker, false, (Power p) => p.CardSource.Card == this.CharacterCard, 3, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // 3 Strain.
            coroutine = this.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => c == this.CharacterCard, 3, DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}