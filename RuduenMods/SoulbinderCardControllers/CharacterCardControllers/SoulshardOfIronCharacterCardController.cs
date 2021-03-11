using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Soulbinder
{
    public class SoulshardOfIronCharacterCardController : SoulbinderSharedMultiCharacterCardController
    {
        public SoulshardOfIronCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddSideTrigger(this.AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageSource.Card != null && !dda.DamageSource.Card.IsHero && dda.DamageSource.Card.IsTarget && dda.Target == this.Card, (DealDamageAction dda) => 1));
            }
        }
    }
}