using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Setback
{
    public class SetbackDoubleOrNothingCharacterCardController : PromoDefaultCharacterCardController
    {
        public SetbackDoubleOrNothingCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 2);
            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            IEnumerator coroutine;

            // Play card.
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false, storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.HeroTurnTaker.Deck.NumberOfCards == 1)
            {
                // One card. Reveal and discard.
                Card revealedCard = this.HeroTurnTaker.Deck.TopCard;
                coroutine = this.GameController.SendMessageAction(revealedCard.AlternateTitleOrTitle + " was the only card in " + this.HeroTurnTaker.Deck.GetFriendlyName() + ". It must be discarded.", Priority.High, this.GetCardSource(), new Card[] { revealedCard });
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCard(this.DecisionMaker, revealedCard, this.HeroTurnTaker.Trash, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Reveal cards.
                List<Card> revealedCards = new List<Card>();
                coroutine = this.GameController.RevealCards(this.DecisionMaker, this.HeroTurnTaker.Deck, 2, revealedCards, true, RevealedCardDisplay.ShowRevealedCards, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // If there are exactly 2 cards, compare keywords.
                if (revealedCards.Count == 2)
                {
                    IEnumerable<string>[] keywordSets = new IEnumerable<string>[]
                    {
                        this.GameController.GetAllKeywords(revealedCards.FirstOrDefault()),
                        this.GameController.GetAllKeywords(revealedCards.LastOrDefault())
                    };
                    if (keywordSets[0].Intersect(keywordSets[1]).Any())
                    {
                        coroutine = this.GameController.MoveCards(this.DecisionMaker, revealedCards, this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else
                    {
                        // Add 2 to the unlucky pool.
                        coroutine = this.GameController.AddTokensToPool(this.CharacterCard.FindTokenPool(TokenPool.UnluckyPoolIdentifier), powerNumeral, this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }

                    if (this.HeroTurnTaker.Revealed.NumberOfCards > 0)
                    {
                        // Discard all remaining cards revealed, if movement failed for some reason.
                        // SelectCardsFromLocationAndMoveThem seems to be the cleanest way to be allowed to control the order.
                        // Note that just MoveCards will probably work fine, since Captain Cosmic and the like use it. But SS Tachyon does something more specific.
                        coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, this.HeroTurnTaker.Revealed, this.HeroTurnTaker.Revealed.NumberOfCards, this.HeroTurnTaker.Revealed.NumberOfCards, new LinqCardCriteria(), new List<MoveCardDestination>() { new MoveCardDestination(this.HeroTurnTaker.Trash) }, isDiscardIfMovingToTrash: true, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }

                // Cleanup. This is only for emergency use - graceful selection for discard order is preferred!
                coroutine = this.CleanupCardsAtLocations(new List<Location>() { this.HeroTurnTaker.Revealed }, this.HeroTurnTaker.Deck, true, isDiscard: true, isReturnedToOriginalLocation: false, cardsInList: revealedCards);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}