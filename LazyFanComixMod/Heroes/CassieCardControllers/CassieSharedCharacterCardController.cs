using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: TEST!

namespace LazyFanComix.Cassie
{
    public abstract class CassieSharedCharacterCardController : HeroCharacterCardController, ICassieRiverSharedCardController
    {

        public CassieSharedCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(GetRiverbankString, null, null);
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return SetupRiverbank();
        }

        private IEnumerator SetupRiverbank()
        {
            IEnumerable<Card> riverCards = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("river") && c.Owner == this.TurnTaker && c.Location == this.TurnTaker.OffToTheSide);
            if (riverCards.Any())
            {
                IEnumerator coroutine;
                coroutine = this.GameController.MoveCards(this.DecisionMaker, riverCards, RiverDeck(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.ShuffleLocation(RiverDeck(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCards(this.DecisionMaker, RiverDeck().GetTopCards(4), Riverbank().UnderLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public string GetRiverbankString()
        {
            if (Riverbank() != null)
            {
                CardController riverCC = this.GameController.FindCardController(Riverbank());
                if (riverCC is RiverbankCardController)
                {
                    return string.Format("The cards under the Riverbank are: {0}", new object[] { ((RiverbankCardController)riverCC).CardsAndCostsUnder() });
                }
            }
            return "The Riverbank is not available.";
        }

        public Location RiverDeck()
        {
            return this.TurnTakerControllerWithoutReplacements.TurnTaker.FindSubDeck("RiverDeck");
        }

        public Card Riverbank()
        {
            return this.TurnTakerControllerWithoutReplacements.TurnTaker.FindCard("Riverbank", false);
        }
    }
}