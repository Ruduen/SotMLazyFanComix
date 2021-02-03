using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class WornBinocularsCardController : CardController
    {
        public WornBinocularsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }
        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
            this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.HeroUsingPower == this.DecisionMaker && upa.Power.CardController.Card.IsPosition && upa.IsSuccessful, ResponseAction, TriggerType.RevealCard, TriggerTiming.After);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        protected IEnumerator ResponseAction(UsePowerAction upa)
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

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == this.HeroTurnTaker;
        }
    }
}