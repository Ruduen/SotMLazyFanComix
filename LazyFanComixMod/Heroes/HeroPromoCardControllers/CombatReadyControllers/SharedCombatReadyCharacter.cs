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

        public static void InitialSetupPutInPlay(CharacterCardController ccc, string[] setupCards)
        {
            IEnumerator coroutine;
            List<Card> cardsToPlay = new List<Card>();
            foreach (string s in setupCards)
            {
                cardsToPlay.Add(GetPreferringDeck(ccc.TurnTaker, s));
            }

            foreach (Card c in cardsToPlay.Where((Card c) => c != null))
            {
                ccc.TurnTaker.MoveCard(c, ccc.TurnTaker.PlayArea);
            }

            ccc.TurnTaker.Deck.ShuffleCards();

            while (ccc.TurnTaker.IsHero && ccc.HeroTurnTaker.NumberOfCardsInHand < 4 && ccc.TurnTaker.Deck.NumberOfCards > 0)
            {
                ccc.TurnTaker.MoveCard(ccc.TurnTaker.Deck.TopCard, ccc.HeroTurnTaker.Hand);
            }
        }

        public static IEnumerator SetFlag(CardController cc)
        {
            cc.AddCardPropertyJournalEntry(SetupDone, true);
            yield break;
        }

        public static string SetupDone { get { return "HeroSetupDone"; } }
    }
}