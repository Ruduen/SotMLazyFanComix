﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class OverclockSystemsCardController : GreyhatSharedNetworkCardController
    {
        public OverclockSystemsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card != null && this.CardsLinksAreNextToHeroes.Contains(dd.DamageSource.Card) && this.CardsLinksAreNextToNonHero.Contains(dd.Target), (DealDamageAction dda) => 1);
        }
    }
}