using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.BreachMage
{
    public class ABreachICardController : BreachMageSharedStableBreachCardController
    {
        public ABreachICardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}