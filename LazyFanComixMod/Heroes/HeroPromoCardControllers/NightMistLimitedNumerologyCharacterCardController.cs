using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.NightMist
{
    public class NightMistLimitedNumerologyCharacterCardController : PromoDefaultCharacterCardController
    {
        public NightMistLimitedNumerologyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 1);
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            IEnumerator coroutine;

            // Reduce Nightmist's damage to hero targets.
            ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(powerNumeral);
            statusEffect.UntilThisTurnIsOver(this.Game);
            statusEffect.TargetCriteria.IsHero = true;
            statusEffect.SourceCriteria.IsSpecificCard = this.CharacterCard;

            coroutine = this.AddStatusEffect(statusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play a card
            coroutine = this.DrawACardOrPlayACard(this.HeroTurnTakerController, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}