using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace RuduenWorkshop.BreachMage
{
    public class BreachIICardController : BreachMageSharedStableBreachCardController
    {
        public BreachIICardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}