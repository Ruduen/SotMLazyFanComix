using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Cassie
{
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

        public override void AddTriggers()
        {
            this.Card.UnderLocation.OverrideIsInPlay = false;
            this.AddTrigger<MoveCardAction>((MoveCardAction m) => m.Origin == this.Riverbank().UnderLocation && m.Destination != this.RiverDeck(), RefillRiverbankResponse, TriggerType.MoveCard, TriggerTiming.After);
            this.AddTrigger<PlayCardAction>((PlayCardAction p) => p.Origin == this.Riverbank().UnderLocation, RefillRiverbankResponse, TriggerType.MoveCard, TriggerTiming.After);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == this.Card || card.Location == this.Card.UnderLocation;
        }

        private IEnumerator RefillRiverbankResponse(PlayCardAction p)
        {
            return RefillRiverbankResponseHelper();
        }

        private IEnumerator RefillRiverbankResponse(MoveCardAction m)
        {
            return RefillRiverbankResponseHelper();
        }

        private IEnumerator RefillRiverbankResponseHelper()
        {
            IEnumerator coroutine;
            Card remainingCard = Riverbank().UnderLocation.Cards.FirstOrDefault();

            // Then, move the top card to the riverbank. Normal empty deck logic should work if they aren't available.
            coroutine = this.GameController.MoveCards(this.HeroTurnTakerController, RiverDeck().GetTopCards(4 - Riverbank().UnderLocation.Cards.Count()), Riverbank().UnderLocation);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}