using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class BottledLightningOldCardController : SharedEquipmentCardController
    {
        public BottledLightningOldCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 1)
            };
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();

            IEnumerator coroutine;

            // Select target(s). 

            coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHero), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Card c in scdResults.Select((SelectCardDecision scd) => scd.SelectedCard))
            {
                // Deal target damage.
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, c), powerNums[1], DamageType.Lightning, powerNums[0], false, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Deal self damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, c), c, powerNums[1], DamageType.Lightning, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Increase next damage based on power numeral.
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(powerNums[2]);
                idse.SourceCriteria.IsSpecificCard = c;
                idse.UntilEndOfNextTurn(this.TurnTaker);
                idse.UntilCardLeavesPlay(c);
                coroutine = this.AddStatusEffect(idse, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            


            
        }
    }
}