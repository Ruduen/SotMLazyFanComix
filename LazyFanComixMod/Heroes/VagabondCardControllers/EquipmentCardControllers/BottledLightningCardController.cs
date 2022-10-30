using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class BottledLightningCardController : SharedEquipmentCardController
    {
        public BottledLightningCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };
            List<GainHPAction> ghaResults = new List<GainHPAction>();

            IEnumerator coroutine;

            // Select target(s). 

            coroutine = this.GameController.SelectAndGainHP(this.DecisionMaker, powerNums[1], false, null, powerNums[0], powerNums[0], false, ghaResults, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Card c in ghaResults.Select((GainHPAction gha) => gha.HpGainer))
            {
                // Increase round of damage based on power numeral.
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(powerNums[2]);
                idse.SourceCriteria.IsSpecificCard = c;
                idse.UntilStartOfNextTurn(this.TurnTaker);
                idse.UntilCardLeavesPlay(c);
                coroutine = this.AddStatusEffect(idse, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}