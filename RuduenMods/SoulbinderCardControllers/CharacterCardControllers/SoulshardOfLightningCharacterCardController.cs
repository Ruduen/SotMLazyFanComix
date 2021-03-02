using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulshardOfLightningCharacterCardController : SoulbinderSharedMultiCharacterCardController
    {
        public SoulshardOfLightningCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsSameCard(this.Card) && !dda.Target.IsHero, 1);
            }
        }
    }
}