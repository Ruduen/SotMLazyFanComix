using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace RuduenWorkshop.BreachMage
{
    public class BreachIVCardController : BreachMageSharedPotentBreachCardController
    {
        public BreachIVCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}