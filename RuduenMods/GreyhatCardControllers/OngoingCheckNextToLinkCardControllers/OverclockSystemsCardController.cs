using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class OverclockSystemsCardController : GreyhatSharedOngoingCheckNextToLinkCardController
    {
        public OverclockSystemsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dd) =>  (dd.DamageSource.Card != null && this.CardsLinksAreNextToGreyhatAndHeroes.Contains(dd.DamageSource.Card)) || this.CardsLinksAreNextToNonHero.Contains(dd.Target), Amount);
        }

        private int Amount(DealDamageAction dda)
        {
            return 1;
        }
    }
}