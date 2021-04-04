using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.BreachMage
{
    public class ABreachIVCardController : BreachMageSharedPotentBreachCardController
    {
        public ABreachIVCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}