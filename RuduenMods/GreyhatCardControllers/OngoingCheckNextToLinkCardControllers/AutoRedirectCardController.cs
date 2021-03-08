using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class AutoRedirectCardController : GreyhatSharedOngoingCheckNextToLinkCardController
    {
        public AutoRedirectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn("GreyhatRedirectOpportunityPresented", null, null, null);
        }

        public override void AddUniqueTriggers()
        {
            this.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => this.CardsLinksAreNextToGreyhatAndHeroes.Contains(dd.Target) && dd.Amount > 0 && dd.Amount <= 3 && this.CardsLinksAreNextToNonHero.Count() > 0, "GreyhatRedirectOpportunityPresented", TargetType.SelectTarget, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), optional: true);
        }
    }
}