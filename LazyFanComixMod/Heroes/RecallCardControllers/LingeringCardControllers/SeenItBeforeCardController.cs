using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class SeenItBeforeCardController : CardController
    {
        public SeenItBeforeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.DrawCards(this.HeroTurnTakerController, 1), TriggerType.DrawCard);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectLocationDecision> sldResults = new List<SelectLocationDecision>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.DiscardFromDeck, (Location l) => true, sldResults, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Location l in sldResults.Select((SelectLocationDecision sld) => sld.SelectedLocation.Location))
            {
                if (l != null)
                {
                    List<Card> storedResultsCard = new List<Card>();
                    coroutine = this.GameController.RevealCards(this.HeroTurnTakerController, l, 1, storedResultsCard, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    Card card = storedResultsCard.FirstOrDefault();
                    if (card != null)
                    {
                        List<MoveCardDestination> list = new List<MoveCardDestination>
                    {
                        new MoveCardDestination(l, false, false, false),
                        new MoveCardDestination(l, true, false, false)
                    };
                        coroutine = this.GameController.SelectLocationAndMoveCard(this.HeroTurnTakerController, card, list, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    coroutine = this.CleanupCardsAtLocations(new List<Location> { l.OwnerTurnTaker.Revealed }, l, cardsInList: storedResultsCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}