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
                    // Increase damage if the direct trigger of the damage was this card.
                    return (from acs in dd.CardSource.AssociatedCardSources
                            where acs.Card != null
                            select acs.Card).Any((Card c) => c == this.Card);
                }
                this.AddIncreaseDamageTrigger(criteria, 1, null, null, false);
            }
        }
    }
}