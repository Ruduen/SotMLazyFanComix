using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.CombatReady
{
    public abstract class CombatReadySharedTurnTakerController : HeroTurnTakerController
    {
        public CombatReadySharedTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected abstract string[] _setupCardIdentifiers { get; }
        protected abstract IEnumerator _drawUntilFour { get; }

        protected Card GetPreferringDeck(string identifier)
        {
            Card[] cards = this.TurnTaker.GetCardsWhere((Card c) => c.Identifier == identifier).ToArray();
            foreach (Card c in cards)
            {
                if (c.Location.IsDeck)
                {
                    return c;
                }
            }
            return cards.FirstOrDefault();
        }

        public override IEnumerator StartGame()
        {
            IEnumerator coroutine;
            List<Card> cardsToPlay = new List<Card>();
            foreach (string s in _setupCardIdentifiers)
            {
                cardsToPlay.Add(GetPreferringDeck(s));
            }

            foreach (Card c in cardsToPlay)
            {
                coroutine = this.GameController.PlayCard(this, c, true, cardSource: new CardSource(this.CharacterCardController));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: new CardSource(this.CharacterCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.NumberOfCardsInHand < 4)
            {
                coroutine = _drawUntilFour;
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}