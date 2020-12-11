using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachIIICardController : BreachMageSharedBreachController
    {
        public BreachIIICardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            // Use default logic for flips.
            base.AddSideTriggers();

            // If on flipped side, also add increase damage trigger.
            if (this.CardWithoutReplacements.IsInPlay && this.CardWithoutReplacements.IsFlipped)
            {
                // Add damage boost if the direct source of the damage trigger was this card.
                bool criteria(DealDamageAction dd)
                {
                    // Increase damage if the spell cast is next to this card.
                    // TODO: Also check if the damage is from a Cast effect!
                    return (dd.CardSource.Card.Location == this.Card.NextToLocation && dd.CardSource.Card.IsSpell);
                }
                ITrigger openTrigger = this.AddIncreaseDamageTrigger(criteria, 1, null, null, false);
                this.AddSideTrigger(openTrigger);
            }
        }
    }
}