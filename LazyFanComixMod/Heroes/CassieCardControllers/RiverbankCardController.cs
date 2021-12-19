using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Cassie
{
    // Token: 0x0200054D RID: 1357
    public class RiverbankCardController : CassieRiverSharedCardController
    {
        public RiverbankCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(() => string.Format("The cards under this are: {0}",
                new object[] { CardsAndCostsUnder() }),
                null, null);
            this.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public string CardsAndCostsUnder()
        {
            string cardStr = "";
            foreach (Card card in this.Card.UnderLocation.Cards)
            {
                if (cardStr != "") { cardStr += ", "; }
                cardStr += card.AlternateTitleOrTitle;
                if (card.FindTokenPool("CassieCostPool").MaximumValue != null)
                {
                    cardStr += " (Aqua Cost " + card.FindTokenPool("CassieCostPool").MaximumValue + ")";
                }
            }
            if (cardStr == "")
            {
                cardStr = "None";
            }
            return cardStr;
        }

        public override void AddStartOfGameTriggers()
        {
            // Start of Game is best for handling potential oddities with OblivAeon, so do Riverbank setup here.

            this.AddStartOfTurnTrigger(
                (tt) => !this.IsPropertyTrue(SharedCombatReadyCharacter.SetupDone),
                (pca) => SharedCombatReadyCharacter.SetFlag(this),
                TriggerType.Hidden
            );
            if (!this.HasBeenSetToTrueThisGame(SharedCombatReadyCharacter.SetupDone))
            {
                SetupRiverbank();
            }
        }
        public override void AddTriggers()
        {
            this.AddTrigger<MoveCardAction>((MoveCardAction m) => m.Origin == this.Riverbank().UnderLocation && m.Destination != this.RiverDeck(), new Func<MoveCardAction, IEnumerator>(this.RefillRiverbankResponse), TriggerType.MoveCard, TriggerTiming.After);
            this.AddTrigger<PlayCardAction>((PlayCardAction p) => p.Origin == this.Riverbank().UnderLocation, new Func<PlayCardAction, IEnumerator>(this.RefillRiverbankResponse), TriggerType.PlayCard, TriggerTiming.After);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == this.Card || card.Location == this.Card.UnderLocation;
        }

        private IEnumerator RefillRiverbankResponse(PlayCardAction p)
        {
            IEnumerator coroutine = RefillRiverbankResponseHelper();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RefillRiverbankResponse(MoveCardAction m)
        {
            IEnumerator coroutine = RefillRiverbankResponseHelper();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RefillRiverbankResponseHelper()
        {
            IEnumerator coroutine;
            Card remainingCard = Riverbank().UnderLocation.Cards.FirstOrDefault();
            //// Move remaining riverbank cards.
            //// Removed during physical revamp to make the process simpler.
            //while (remainingCard != null)
            //{
            //    coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, remainingCard, RiverDeck(), toBottom: true, evenIfIndestructible: true);
            //    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            //    remainingCard = Riverbank().UnderLocation.Cards.FirstOrDefault();
            //}

            // Confirm number of cards to move.
            int cardsToMove = 4 - Riverbank().UnderLocation.Cards.Count();

            // Then, move the top card to the riverbank. Normal empty deck logic should work if they aren't available.
            coroutine = this.GameController.MoveCards(this.HeroTurnTakerController, RiverDeck().GetTopCards(cardsToMove), Riverbank().UnderLocation);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        private void SetupRiverbank()
        {
            IEnumerable<Card> riverCards = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("river") && c.Owner == this.TurnTaker && c.Location == this.TurnTaker.OffToTheSide);
            if(riverCards.Count() > 0)
            {
                Location riverDeck = this.TurnTaker.FindSubDeck("RiverDeck");
                foreach (Card c in riverCards)
                {
                    this.TurnTaker.MoveCard(c, riverDeck);
                }
                riverDeck.ShuffleCards();
                for (int i = 0; i < 4; i++)
                {
                    this.TurnTaker.MoveCard(riverDeck.TopCard, this.Card.UnderLocation);
                }
            }
        }
    }
}