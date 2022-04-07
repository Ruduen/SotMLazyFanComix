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
            new Function(this.HeroTurnTakerController, "Move the top card of your deck under this card", SelectionType.MoveCardToUnderCard,
                () => this.GameController.MoveCards(this.HeroTurnTakerController, this.HeroTurnTaker.Deck, this.Card.UnderLocation, 1),
                this.TurnTaker.Deck.HasCards || this.TurnTaker.Trash.HasCards,  this.TurnTaker.Name + " play a card from under " + this.CardWithoutReplacements.Title + ", so they must move the top card of their deck to under themselves."),
            new Function(this.HeroTurnTakerController, "Play a card from under this card", SelectionType.PlayCard,
                () => this.GameController.SelectAndPlayCard(this.HeroTurnTakerController, (Card c) => c.Location == this.CardWithoutReplacements.UnderLocation, cardSource: this.GetCardSource()),
                this.CardWithoutReplacements.UnderLocation.HasCards,this.TurnTaker.Name + " cannot move the top card of their deck to under themselves, so they must play a card from under themselves.")
            };

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot move the top card of their deck to under themselves or play a card from under " + this.CardWithoutReplacements.Title + ", so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            IEnumerator coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}