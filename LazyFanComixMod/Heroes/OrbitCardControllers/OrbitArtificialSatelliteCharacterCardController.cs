using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace LazyFanComix.Orbit
{
    public class OrbitArtificialSatelliteCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public OrbitArtificialSatelliteCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };

            IEnumerator coroutine;

            coroutine = this.DrawCards(this.DecisionMaker, powerNumerals[0]);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumerals[1]);
            rdse.TargetCriteria.IsSpecificCard = this.CharacterCard;
            rdse.UntilStartOfNextTurn(this.TurnTaker);

            coroutine = this.GameController.AddStatusEffect(rdse, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace Incap with something more unique!
    }
}