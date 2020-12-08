using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Cassie
{
    public class CassieTurnTakerController : HeroTurnTakerController
    {
        public CassieTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            // Placeholder iteration - since side decks can't currently display card backs, this is necessary to make the cards show.
            {
                IEnumerator coroutine;
                Location riverDeck = this.TurnTaker.FindSubDeck("RiverDeck");

                // At the start of game, move all River cards into River deck. (This best preserves the 'identity' of the cards.)
                int handSize = this.NumberOfCardsInHand;
                IEnumerable<Card> riverCards = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("river") && c.Owner == this.TurnTaker);
                coroutine = this.GameController.BulkMoveCards(this, riverCards, riverDeck);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Then shuffle.
                coroutine = this.GameController.ShuffleLocation(riverDeck);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Then, move the top four to the riverbank. That should already exist due to being a non-real card.
                coroutine = this.GameController.MoveCards(this, riverDeck.GetTopCards(4), this.TurnTaker.FindCard("Riverbank", false).UnderLocation);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}