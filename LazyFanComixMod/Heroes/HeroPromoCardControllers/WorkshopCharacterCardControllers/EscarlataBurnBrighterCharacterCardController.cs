using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Escarlata
{
    public class EscarlataBurnBrighterCharacterCardController : PromoDefaultCharacterCardController
    {
        public EscarlataBurnBrighterCharacterCardController(Card card, TurnTakerController turnTakerController)
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
            IEnumerator coroutine;

            OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(this.CardWithoutReplacements, "FireBoostResponse", "Whenever " + this.Card.Title + "deals fire damage to themselves, prevent it and increase the next damage they deal to a non-hero target.", new TriggerType[] { TriggerType.CancelAction, TriggerType.IncreaseDamage }, this.TurnTaker, this.Card, powerNums);
            oddse.SourceCriteria.IsSpecificCard = this.Card;
            oddse.TargetCriteria.IsSpecificCard = this.Card;
            oddse.DamageTypeCriteria.AddType(DamageType.Fire);
            oddse.UntilEndOfNextTurn(this.TurnTaker);
            oddse.UntilTargetLeavesPlay(this.Card);
            oddse.BeforeOrAfter = BeforeOrAfter.Before;

            coroutine = this.GameController.AddStatusEffect(oddse, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), powerNums[2], DamageType.Fire, powerNums[1], false, powerNums[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public IEnumerator FireBoostResponse(DealDamageAction dda, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            IEnumerator coroutine;

            int num = 1;
            if (powerNumerals != null)
            {
                num = powerNumerals[0];
            }

            coroutine = this.CancelAction(dda);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(num);
            idse.Identifier = "IncreaseNextDamageDealtBy" + this.Card.ToString() + "ThanksTo" + this.Card.Identifier;
            idse.NumberOfUses = 1;
            idse.SourceCriteria.IsSpecificCard = this.Card;
            idse.TargetCriteria.IsHero = false;
            coroutine = this.GameController.AddStatusEffect(idse, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}