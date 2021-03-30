using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.TheVisionary
{
    public class TheVisionaryProphecyCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheVisionaryProphecyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            List<Function> list = new List<Function>()
            {
            new Function(this.DecisionMaker, "Move the top card of your deck under this card.", SelectionType.MoveCardToUnderCard,
                () => this.GameController.MoveCards(this.DecisionMaker, this.HeroTurnTaker.Deck, this.Card.UnderLocation, 1),
                this.TurnTaker.Deck.HasCards || this.TurnTaker.Trash.HasCards, this.TurnTaker.Name + " cannot move the top card of their deck to under themselves, so they must play a card from under themselves."),
            new Function(this.DecisionMaker, "Play a card from under this card.", SelectionType.DiscardCard,
                () => this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.Card.UnderLocation || c.Location == this.Card.BelowLocation, cardSource: this.GetCardSource()),
                this.Card.UnderLocation.HasCards || this.Card.BelowLocation.HasCards, this.TurnTaker.Name + " play a card from under themselves, so they must move the top card of their deck to under themselves.")
            };

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot move the top card of their deck to under themselves or play a card from under themselves, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            IEnumerator coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}