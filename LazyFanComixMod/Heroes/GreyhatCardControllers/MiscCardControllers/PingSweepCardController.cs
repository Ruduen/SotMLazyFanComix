using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Greyhat
{
    public class PingSweepCardController : CardController
    {
        public PingSweepCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<RevealCardsAction> rcaResults = new List<RevealCardsAction>();
            IEnumerator coroutine;

            // Search for and reveal 2 links.
            coroutine = this.GameController.RevealCards(this.HeroTurnTakerController, this.TurnTaker.Deck, (Card c) => c.IsLink, 2, rcaResults, RevealedCardDisplay.None, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Move them into play.
            coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, this.TurnTaker.Revealed, 2, 2, new LinqCardCriteria((Card c) => c.IsLink && rcaResults.SelectMany((RevealCardsAction rc) => rc.RevealedCards).Contains(c), "link"), new MoveCardDestination[] { new MoveCardDestination(this.TurnTaker.PlayArea) }, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Clean up revealed cards.
            coroutine = this.CleanupRevealedCards(this.TurnTaker.Revealed, this.TurnTaker.Deck, shuffleAfterwards: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}