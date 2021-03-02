using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public abstract class GreyhatSharedCheckNextToLinkCardController : CardController
    {
        public IEnumerable<Card> CardsLinksAreNextTo
        {
            get
            {
                // Find all links in play. Check for any which are next to a card. Return those owner cards without duplicates.
                return this.GameController.FindCardsWhere((Card c) => c.IsLink && c.IsInPlayAndNotUnderCard).Where((Card c) => c.Location.IsNextToCard).Select((Card c) => c.Location.OwnerCard).Distinct();
            }
        }

        public GreyhatSharedCheckNextToLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}