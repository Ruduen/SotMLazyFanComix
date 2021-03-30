using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.VoidGuardTheIdealist
{
    public class VoidGuardTheIdealistStreamOfConsciousnessCharacterCardController : PromoDefaultCharacterCardController
    {
        public VoidGuardTheIdealistStreamOfConsciousnessCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // Create list of potential locations to play from - under/below concepts. Yes, both must be checked based on Omnicannon.
            IEnumerable<Card> conceptCards = this.FindCardsWhere((Card c) => c.IsConcept && c.IsInPlayAndHasGameText);
            List<Location> underConceptLocations = new List<Location>();
            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            underConceptLocations.AddRange(conceptCards.Select((Card c) => c.UnderLocation));
            underConceptLocations.AddRange(conceptCards.Select((Card c) => c.BelowLocation));

            // Play one of your cards from under/below a concepts.
            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => underConceptLocations.Contains(c.Location) && c.Owner == this.HeroTurnTaker, storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If the play fails for whatever reason, topdeck.
            if (storedResults.Count == 0 || !storedResults.FirstOrDefault().WasCardPlayed)
            {
                if (this.HeroTurnTaker.Deck.IsEmpty)
                {
                    coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " cannot play the top card of their deck, so" + this.Card.AlternateTitleOrTitle + " has no effect.", Priority.Medium, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, numberOfCards: 1, showMessage: true, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}