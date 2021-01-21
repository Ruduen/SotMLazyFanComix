using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Unity
{
    public class UnityNewPlansCharacterCardController : PromoDefaultCharacterCardController
    {
        public UnityNewPlansCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = {
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 2)
            };
            List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
            List<Card> consideredCards = new List<Card>();
            int? lowestHP = null;

            IEnumerator coroutine;

            // Discard 3 cards.
            coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, powerNumerals[0], false, powerNumerals[0], storedResultsDiscard, allowAutoDecide: true, cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (DiscardCardAction dca in storedResultsDiscard.Where((DiscardCardAction a) => a.WasCardDiscarded))
            {
                consideredCards.Add(dca.CardToDiscard);
            }
            foreach (Card card in consideredCards.Where((Card c) => c.IsMechanicalGolem))
            {
                if (lowestHP == null || card.MaximumHitPoints < lowestHP) { lowestHP = card.MaximumHitPoints; }
            }

            if (lowestHP == null)
            {
                coroutine = this.GameController.SendMessageAction("No appropriate Mechanical Golems were discarded, so none cannot be played.", Priority.Medium, this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Play one of the mechanical golems with the lowest HP.
                coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, consideredCards.Where((Card c) => c.IsMechanicalGolem && c.MaximumHitPoints == lowestHP), cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Draw 2 cards.
            coroutine = this.DrawCards(this.HeroTurnTakerController, powerNumerals[1]);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}