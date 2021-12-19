using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.AbsoluteZero;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AbsoluteZero
{
    public class AbsoluteZeroCombatReadyCharacterCardController : AbsoluteZeroCharacterCardController
    {
        public AbsoluteZeroCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

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

        public override void AddStartOfGameTriggers()
        {
            InitialSetup();
        }

        public void InitialSetup()
        {
            if (this.IsPropertyTrue(SetupDone))
            {
                return;
            }

            this.AddCardPropertyJournalEntry(SetupDone, true);
            string[] setupCards = { "IsothermicTransducer", "GlacialStructure" };
            IEnumerator coroutine;
            List<Card> cardsToPlay = new List<Card>();
            foreach (string s in setupCards)
            {
                cardsToPlay.Add(GetPreferringDeck(s));
            }

            foreach (Card c in cardsToPlay)
            {
                this.TurnTaker.MoveCard(c, this.TurnTaker.PlayArea);
            }

            while (this.HeroTurnTaker.NumberOfCardsInHand < 4 && this.TurnTaker.Deck.NumberOfCards > 0)
            {
                this.TurnTaker.MoveCard(this.TurnTaker.Deck.TopCard, this.HeroTurnTaker.Hand);
            }

        }

        private const string SetupDone = "SetupDone";


    }
}