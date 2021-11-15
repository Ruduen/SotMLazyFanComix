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

    public abstract class CassieRiverSharedCardController : CardController
    {
        // TO DO: If this doesn't work cleanly, remove the entire static variable!

        protected static Location _riverDeck;
        protected static Card _riverbank;
        protected static TurnTakerController _turnTakerController;

        public CassieRiverSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Set these up on initialization, so Guise doesn't do bad things.
            _turnTakerController = TurnTakerController;
            _riverbank = null;
            _riverDeck = null;
        }

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(this.Card.Owner.Trash, false, false, false);  // All cards should go into the hero trash during trashing, not river deck.
        }

        protected string GetRiverbankString()
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
            if (CassieRiverSharedCardController._riverDeck == null)
            {
                // TODO: These must always find Cassie's river deck. Even if Guise is using things!
                CassieRiverSharedCardController._riverDeck = _turnTakerController.TurnTaker.FindSubDeck("RiverDeck");
            }
            return CassieRiverSharedCardController._riverDeck;
        }

        public Card Riverbank()
        {
            if (CassieRiverSharedCardController._riverbank == null)
            {
                CassieRiverSharedCardController._riverbank = _turnTakerController.TurnTaker.FindCard("Riverbank", false);
            }
            return CassieRiverSharedCardController._riverbank;
        }
    }
}