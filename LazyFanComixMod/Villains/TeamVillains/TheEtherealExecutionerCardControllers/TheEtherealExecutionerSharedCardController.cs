using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public abstract class TheEtherealExecutionerSharedCardController : CardController
    {
        public TheEtherealExecutionerSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public int CountObservationCards()
        {
            return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("observation") && c.Owner == this.TurnTaker, true).Count();
        }
    }
}