using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace RuduenWorkshop.Trailblazer
{
    public class ThrowToTheTrailsCardController : CardController
    {
        public ThrowToTheTrailsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}