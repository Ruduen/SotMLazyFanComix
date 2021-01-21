using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Inquirer
{
    public class TheLieTheyTellThemselvesCardController : CardController
    {
        public TheLieTheyTellThemselvesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Add trigger for play Distortion. Must be a played distortion, and the distortion must've been played next to a non-hero target.
            this.AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardToPlay.IsDistortion && pca.WasCardPlayed && pca.CardToPlay.Location.IsNextToCard && !pca.CardToPlay.Location.OwnerCard.IsHero && pca.CardToPlay.Location.OwnerCard.IsTarget, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(PlayCardAction pca)
        {
            // Viability of target was checked with trigger, so only worry about the card.
            Card nextToCard = pca.CardToPlay.Location.OwnerCard;

            // That target damages themselves.
            IEnumerator coroutine = this.DealDamage(nextToCard, nextToCard, 1, DamageType.Psychic, false, false, false, null, null, null, false, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Draw 2 cards.
            int powerNumeral = this.GetPowerNumeral(0, 2);
            IEnumerator coroutine = this.DrawCards(this.DecisionMaker, powerNumeral);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}