using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace LazyFanComix.Vagabond
{
    public class VagabondCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public VagabondCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };

            // Deal <a> target <b> damage.
            return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, 0, cardSource: this.GetCardSource());

        }

        // TODO: Replace Incap with something more unique!
    }
}