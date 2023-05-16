using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class ThriThirdPowerCardController : CardController
    {
        private bool _isThirdPower;
        private UsePowerAction _thirdPowerUpa;

        public bool isThirdPower
        {
            get { return _isThirdPower; }
        }

        public ThriThirdPowerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
            _isThirdPower = false;
            _thirdPowerUpa = null;
        }

        //protected bool checkThirdPower()
        //{
        //     Check if this is the third power.
        //     NOTE: This is currently unstable, since it only checks if 2 powers were used, which may not be accurate if another power was used while this one is still resolving, such as
        //     from an earlier trigger. But I haven't figured out the best way to sanely future-proof for the 'use multiple times' case. That's a future goal, not a playtesting one.
        //    IEnumerable<UsePowerJournalEntry> powersThisTurn = this.Journal.UsePowerEntriesThisTurn();
        //    if (powersThisTurn.Count() == 3 && powersThisTurn.ElementAt(2).CardSource == this.Card)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        // Use trigger on power use to note that something is the third power use, since that has better potential to handle ordering given timing point.
        public override void AddTriggers()
        {
            this.AddTrigger<UsePowerAction>(checkIsThirdPower, setIsThirdPower, TriggerType.HiddenLast, TriggerTiming.Before);
            this.AddTrigger<UsePowerAction>(checkWasThirdPower, clearIsThirdPower, TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false);
        }

        private bool checkIsThirdPower(UsePowerAction upa)
        {
            return (this == upa.Power.CardController && this.Journal.UsePowerEntriesThisTurn().Count() == 2);
        }

        //private bool checkIsThirdPower(UsePowerAction upa)
        //{
        //    if (this.GameController.GetAllPowersForCardController(this).Contains(upa.Power)) { return false; }
        //    IEnumerable<UsePowerJournalEntry> powersThisTurn = this.Journal.UsePowerEntriesThisTurn();
        //    if (powersThisTurn.Count() == 3 && powersThisTurn.ElementAt(2).CardSource == this.Card)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

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

    }
}