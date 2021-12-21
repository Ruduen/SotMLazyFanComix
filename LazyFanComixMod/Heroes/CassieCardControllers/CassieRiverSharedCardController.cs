using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Cassie
{
    public interface ICassieRiverSharedCardController
    {
        public Location RiverDeck();

        public Card Riverbank();

        public string GetRiverbankString();
    }

    public abstract class CassieRiverSharedCardController : CardController, ICassieRiverSharedCardController
    {
        public CassieRiverSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(this.Card.Owner.Trash, false, false, false);  // All cards should go into the hero trash during trashing, not river deck.
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