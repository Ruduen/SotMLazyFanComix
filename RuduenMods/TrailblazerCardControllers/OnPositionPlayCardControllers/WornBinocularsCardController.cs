using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class WornBinocularsCardController : TrailblazerOnPlayPositionCardController
    {
        public WornBinocularsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        protected override IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => true, storedResults, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            Location deck = base.GetSelectedLocation(storedResults);
            if (deck != null)
            {
                List<Card> storedResultsCard = new List<Card>();
                coroutine = this.GameController.RevealCards(this.DecisionMaker, deck, 1, storedResultsCard, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                Card card = storedResultsCard.FirstOrDefault();
                if (card != null)
                {
                    List<MoveCardDestination> list = new List<MoveCardDestination>
                    {
                        new MoveCardDestination(deck, false, false, false),
                        new MoveCardDestination(deck, true, false, false)
                    };
                    coroutine = this.GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, list, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                coroutine = this.CleanupCardsAtLocations(new List<Location> { deck.OwnerTurnTaker.Revealed }, deck, cardsInList: storedResultsCard);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected override TriggerType ResponseTriggerType()
        {
            return TriggerType.RevealCard;
        }
    }
}