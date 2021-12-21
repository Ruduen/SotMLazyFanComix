using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.BreachMage
{
    public abstract class BreachMageSharedStableBreachCardController : BreachMageSharedBreachController
    {
        public BreachMageSharedStableBreachCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UniquePower()
        {
            // Draw a card or play a card.
            return this.DrawACardOrPlayACard(this.DecisionMaker, true);
        }
    }
}