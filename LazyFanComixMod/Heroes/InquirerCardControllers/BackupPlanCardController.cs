﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace LazyFanComix.Inquirer
{
    public class BackupPlanCardController : CardController
    {
        public BackupPlanCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };

            IEnumerator coroutine;

            // Deal Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            //coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[1]);

            // Heal.
            coroutine = this.GameController.GainHP(this.CharacterCard, powerNumerals[2], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            // Discard card.
            coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidDiscardCards(storedResults))
            {
                // If you do, draw a card.
                coroutine = this.DrawCard(this.HeroTurnTaker, false, null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}