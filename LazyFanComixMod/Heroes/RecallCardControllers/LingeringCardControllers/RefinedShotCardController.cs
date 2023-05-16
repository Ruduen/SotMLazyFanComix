using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RefinedShotCardController : CardController
    {
        public RefinedShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger((UsePowerAction p) => p.Power != null && p.Power.TurnTakerController == this.TurnTakerController && p.IsSuccessful && (p.CardSource == null || !this.GameController.IsInhibited(p.CardSource.CardController, p)), this.DamageResponse, new TriggerType[]
            {
                TriggerType.DealDamage
            }, TriggerTiming.After);
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

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), numerals[1], DamageType.Projectile, numerals[0], false, numerals[0], storedResultsDecisions: scdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Card c in scdResults.Select((SelectCardDecision scd) => scd.SelectedCard))
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(numerals[2]);
                idse.SourceCriteria.IsSpecificCard = this.CharacterCard;
                idse.TargetCriteria.IsSpecificCard = c;
                idse.UntilTargetLeavesPlay(this.CharacterCard);
                idse.UntilTargetLeavesPlay(c);
                idse.UntilEndOfNextTurn(this.TurnTaker);

                coroutine = this.AddStatusEffect(idse, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator DamageResponse(UsePowerAction upa)
        {
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.TurnTaker.CharacterCard), 1, DamageType.Projectile, 1, false, 0, storedResultsDecisions: scdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResults.Count > 0 && scdResults.Any((SelectCardDecision scd) => scd.SelectedCard != null))
            {
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.TurnTaker.CharacterCard), this.TurnTaker.CharacterCard, 1, DamageType.Projectile, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}