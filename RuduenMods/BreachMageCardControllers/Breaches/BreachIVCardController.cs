using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachIVCardController : BreachMageSharedBreachController
    {
        public BreachIVCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            bool criteria(DealDamageAction dd)
            {
                // Increase damage if the spell cast is next to this card.
                // TODO: Also check if the damage is from a Cast effect!
                return (dd.CardSource.Card.Location == this.Card.NextToLocation && dd.CardSource.Card.IsSpell);
            }
            this.AddTrigger(this.AddIncreaseDamageTrigger(criteria, 1, null, null, false));
        }
    }
}