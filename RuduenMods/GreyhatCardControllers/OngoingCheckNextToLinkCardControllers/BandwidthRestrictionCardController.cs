using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class BandwidthRestrictionCardController : GreyhatSharedOngoingCheckNextToLinkCardController
    {
        public BandwidthRestrictionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddReduceDamageTrigger((DealDamageAction dd) => this.CardsLinksAreNextToGreyhatAndHeroes.Contains(dd.Target) || this.CardsLinksAreNextToNonHero.Contains(dd.DamageSource.Card), Amount);
        }

        private int Amount(DealDamageAction dda)
        {
            return 1;
        }
    }
}