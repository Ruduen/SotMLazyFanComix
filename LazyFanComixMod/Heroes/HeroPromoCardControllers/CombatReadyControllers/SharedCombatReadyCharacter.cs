using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Shared
{
    public static class SharedCombatReadyCharacter
    {
        public static Card GetPreferringDeck(TurnTaker tt, string identifier)
        {
            Card[] cards = tt.GetCardsWhere((Card c) => c.Identifier == identifier).ToArray();
            foreach (Card c in cards)
            {
                if (c.Location.IsDeck)
                {
                    return c;
                }
            }
            return cards.FirstOrDefault();
        }

        public static IEnumerator InitialSetupPutInPlay(CardController cc, string[] setupCards)
        {
            IEnumerator coroutine;
            List<Card> cardsToPlay = new List<Card>();

            foreach (string s in setupCards)
            {
                cardsToPlay.Add(GetPreferringDeck(cc.TurnTaker, s));
            }

            coroutine = cc.GameController.MoveCards(cc.DecisionMaker, cardsToPlay, cc.TurnTaker.PlayArea, false, true, cardSource: cc.GetCardSource());
            if (cc.UseUnityCoroutines) { yield return cc.GameController.StartCoroutine(coroutine); } else { cc.GameController.ExhaustCoroutine(coroutine); }

            coroutine = cc.GameController.ShuffleLocation(cc.TurnTaker.Deck, cardSource: cc.GetCardSource());
            if (cc.UseUnityCoroutines) { yield return cc.GameController.StartCoroutine(coroutine); } else { cc.GameController.ExhaustCoroutine(coroutine); }

            if (cc.TurnTaker.IsHero && cc.HeroTurnTaker.NumberOfCardsInHand < 4 && cc.TurnTaker.Deck.NumberOfCards > 0)
            {
                coroutine = cc.GameController.MoveCards(cc.DecisionMaker, cc.TurnTaker.Deck.GetTopCards(4 - cc.HeroTurnTaker.NumberOfCardsInHand), cc.HeroTurnTaker.Hand, cardSource: cc.GetCardSource());
                if (cc.UseUnityCoroutines) { yield return cc.GameController.StartCoroutine(coroutine); } else { cc.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}