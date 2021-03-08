using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class ShockTherapyCardController : GreyhatSharedCheckNextToLinkCardController
    {
        public ShockTherapyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> gainedHPCards = new List<Card>();
            List<Card> didDamageCards = new List<Card>();

            // Greyhat heals.
            coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Healing Portion.
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => this.CardsLinksAreNextToOtherHeroes.Contains(c), 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Damage Portion.
            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), 2, DamageType.Energy, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}