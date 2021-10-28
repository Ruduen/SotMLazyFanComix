using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AkashThriya
{
    public class AkashThriyaPlantEssenceCharacterCardController : PromoDefaultCharacterCardController
    {
        public AkashThriyaPlantEssenceCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scdResult = new List<SelectCardDecision>();
            Location envDeck = this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Deck;

            // Shuffle the environment deck. 
            coroutine = this.GameController.ShuffleLocation(envDeck, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select a card from hand. 
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardUnderTopCardOfDeck, this.HeroTurnTaker.Hand.Cards, scdResult, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResult?.FirstOrDefault()?.SelectedCard != null)
            {
                Card card = scdResult.FirstOrDefault().SelectedCard;
                Card topCard = envDeck.TopCard;

                string message = string.Format("{0} moves {1} under the top card of the environment deck.", this.Card.Title, Card.Title);
                coroutine = this.GameController.SendMessageAction(message, Priority.Medium, cardSource: this.GetCardSource(), new Card[] { card }, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Move it on top of the environment deck, then move the stored top card of the environment deck.

                coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, card, envDeck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, topCard, envDeck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Draw.
            coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}