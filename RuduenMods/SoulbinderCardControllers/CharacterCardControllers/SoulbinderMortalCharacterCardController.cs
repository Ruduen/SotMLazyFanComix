using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderMortalCharacterCardController : SoulbinderSharedInstructionsCharacterCardController
    {
        public string str;

        public SoulbinderMortalCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        // TODO: Replace Incap with something more unique!
    }
}