﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class ShockTherapyCardController : GreyhatSharedNetworkCardController
    {
        public ShockTherapyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator UniquePlay()
        {
            IEnumerator coroutine;
            List<Card> gainedHPCards = new List<Card>();
            List<Card> didDamageCards = new List<Card>();
            // Healing Portion.
            coroutine = this.GameController.GainHP(this.HeroTurnTakerController, (Card c) => this.CardsLinksAreNextToHeroes.Contains(c), 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Damage Portion.
            coroutine = this.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), 2, DamageType.Lightning, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}