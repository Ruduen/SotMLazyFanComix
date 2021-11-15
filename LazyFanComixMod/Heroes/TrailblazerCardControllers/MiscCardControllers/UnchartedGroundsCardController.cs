﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public class UnchartedGroundsCardController : CardController
    {
        public UnchartedGroundsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Shuffle Trash into Deck.
            coroutine = this.GameController.ShuffleTrashIntoDeck(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Reveal two positions, play one, shuffle the others.
            coroutine = this.RevealCards_SelectSome_MoveThem_ReturnTheRest(this.HeroTurnTakerController, this.HeroTurnTakerController, this.TurnTaker.Deck, (Card c) => c.IsPosition, 2, 1, false, true, false, "positions");
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Up to 2 Targets 1 Fire.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Fire, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}