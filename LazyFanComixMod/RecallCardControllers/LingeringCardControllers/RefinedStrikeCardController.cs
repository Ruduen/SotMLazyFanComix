using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RefinedStrikeCardController : CardController
    {
        public RefinedStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };
            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), numerals[1], DamageType.Projectile, numerals[0], false, numerals[1], storedResultsDecisions: scdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach(Card c in scdResults.Select((SelectCardDecision scd) => scd.SelectedCard)){
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(numerals[2]);
                idse.SourceCriteria.IsSpecificCard = this.CharacterCard;
                idse.TargetCriteria.IsSpecificCard = c;
                idse.UntilTargetLeavesPlay(this.CharacterCard);
                idse.UntilTargetLeavesPlay(c);
                idse.UntilStartOfNextTurn(this.FindEnvironment().TurnTaker);

                coroutine = this.AddStatusEffect(idse, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}