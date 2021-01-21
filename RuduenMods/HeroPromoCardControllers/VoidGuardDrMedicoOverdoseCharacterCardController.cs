using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.VoidGuardDrMedico
{
    public class VoidGuardDrMedicoOverdoseCharacterCardController : PromoDefaultCharacterCardController
    {
        public VoidGuardDrMedicoOverdoseCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 2), // Number of targets.
                this.GetPowerNumeral(1, 1), // Amount of healing.
                this.GetPowerNumeral(2, 4) // Amount of damage.
            };

            List<GainHPAction> storedResults = new List<GainHPAction>();

            IEnumerator coroutine;

            // Heal 2 targets.
            coroutine = this.GameController.SelectAndGainHP(this.DecisionMaker, powerNumerals[1], numberOfTargets: powerNumerals[0], requiredDecisions: powerNumerals[0], storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal damage to each full HP target. They only had to be selected - no healing is necessary.
            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => storedResults.Select((GainHPAction a) => a.HpGainer).Contains(c) && c.HitPoints >= c.MaximumHitPoints, powerNumerals[2], DamageType.Toxic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}