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
            int[] powerNums = new int[] { this.GetPowerNumeral(0, 2) };
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            // Shuffle the environment deck. 
            coroutine = this.GameController.ShuffleLocation(this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Deck, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<Function> list = new List<Function>()
            {
                new Function(this.HeroTurnTakerController, "Move a card from your hand to under the top " + powerNums[0] + "cards of the Environment deck", SelectionType.MoveCard, () => this.PutCardInEnvDeck(powerNums[0]), this.HeroTurnTaker.Hand.Cards.Count() > 0, this.TurnTaker.Name + " cannot draw any cards, so they must move a card from their hand to the Environment deck."),
                new Function(this.HeroTurnTakerController, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController) || this.HeroTurnTaker.HasCardsInHand, this.TurnTaker.Name + " cannot move a card from their hand to the Environment deck, so they must draw a card.")
            };
            sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " draw any cards or move any cards to the Environment deck, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }

        private IEnumerator PutCardInEnvDeck(int numeral)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scdResult = new List<SelectCardDecision>();
            Location envDeck = this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Deck;

            // Select a card from hand. 
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardUnderTopCardOfDeck, this.HeroTurnTaker.Hand.Cards, scdResult, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResult?.FirstOrDefault()?.SelectedCard != null)
            {
                Card card = scdResult.FirstOrDefault().SelectedCard;
                Card[] topCards = envDeck.GetTopCards(numeral).Reverse().ToArray();

                string message = string.Format("{0} moves {1} under the top " + numeral + " cards of the environment deck.", this.Card.Title, Card.Title);
                coroutine = this.GameController.SendMessageAction(message, Priority.Medium, cardSource: this.GetCardSource(), new Card[] { card }, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Move it on top of the environment deck, then move the stored top card of the environment deck.

                coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, card, envDeck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCards(this.HeroTurnTakerController, topCards, envDeck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}