﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteQuickShotCharacterCardController : PromoDefaultCharacterCardController
    {
        public ExpatrietteQuickShotCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(this.GetPowerNumeral(0, 1));
            reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = this.CharacterCard;
            reduceDamageStatusEffect.UntilThisTurnIsOver(this.Game);
            coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.HeroTurnTaker.Deck.IsEmpty)
            {
                coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " cannot play the top card of their deck, so" + this.Card.AlternateTitleOrTitle + " has no effect.", Priority.Medium, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.PlayTopCard(this.HeroTurnTakerController, this.TurnTakerController, numberOfCards: 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.AdditionalPhaseActionThisTurn(this.HeroTurnTaker, Phase.UsePower, 1, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}