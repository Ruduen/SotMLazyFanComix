using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Thri
{
    public class ThriCharacterCardController : PromoDefaultCharacterCardController
    {
        private bool _isThirdPower;
        private UsePowerAction _thirdPowerUpa;

        public ThriCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
            _isThirdPower = false;
            _thirdPowerUpa = null;
        }
        public override void AddTriggers()
        {
            this.AddTrigger<UsePowerAction>(checkIsThirdPower, setIsThirdPower, TriggerType.HiddenLast, TriggerTiming.Before);
            this.AddTrigger<UsePowerAction>(checkWasThirdPower, clearIsThirdPower, TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false);
        }

        private bool checkIsThirdPower(UsePowerAction upa)
        {
            return (this == upa.Power.CardController && this.Journal.UsePowerEntriesThisTurn().Count() == 2);
        }

        private IEnumerator setIsThirdPower(UsePowerAction upa)
        {
            this._isThirdPower = true;
            this._thirdPowerUpa = upa;
            yield break;
        }

        private bool checkWasThirdPower(UsePowerAction upa)
        {
            return (this._isThirdPower && this._thirdPowerUpa == upa);
        }

        private IEnumerator clearIsThirdPower(UsePowerAction upa)
        {
            this._isThirdPower = false;
            this._thirdPowerUpa = null;
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(1, 3)
            };

            // Deal <a> target <b> damage.
            IEnumerator coroutine;

            // Trigger to increase damage by 3 if appropriate.
            ITrigger tempIncrease = null;

            // Check if this is the third power.
            if (this._isThirdPower)
            {
                tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, powerNumerals[2]);
            }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (tempIncrease != null)
            {
                this.RemoveTrigger(tempIncrease);
            }
        }

        //public override IEnumerator UsePower(int index = 0)
        //{
        //    int[] powerNumerals = new int[]
        //    {
        //        this.GetPowerNumeral(0, 1),
        //        this.GetPowerNumeral(1, 1),
        //        this.GetPowerNumeral(1, 3)
        //    };

        //    // Deal <a> target <b> damage.
        //    IEnumerator coroutine;
        //    // Trigger to increase damage by 2 per cover card.
        //    ITrigger tempIncrease = null;

        //    // Check if this is the third power.
        //    // NOTE: This is currently unstable, since it only checks if 2 powers were used, which may not be accurate if another power was used while this one is still resolving, such as
        //    // from an earlier trigger. But I haven't figured out the best way to sanely future-proof for the 'use multiple times' case. That's a future goal, not a playtesting one.
        //    IEnumerable<UsePowerJournalEntry> powersThisTurn = this.Journal.UsePowerEntriesThisTurn();
        //    if (powersThisTurn.Count() == 3 && powersThisTurn.ElementAt(2).CardSource == this.Card)
        //    {
        //        tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, powerNumerals[2]);
        //    }

        //    coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
        //    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        //    if (tempIncrease != null)
        //    {
        //        this.RemoveTrigger(tempIncrease);
        //    }
        //}

        // TODO: Replace Incap with something more unique!
    }
}