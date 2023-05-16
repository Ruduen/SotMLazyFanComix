using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class RuleOfThreeCardController : CardController
    {
        private ITrigger tempIncrease;
        private UsePowerAction storedUPA;
        public RuleOfThreeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == this.TurnTaker, Phase.UsePower, 1);
            this.AddTrigger<UsePowerAction>(isHeroUsingThirdPower, increasePowerDamage, TriggerType.HiddenLast, TriggerTiming.Before);
            this.AddTrigger<UsePowerAction>(isSamePower, resetPowerDamage, TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false);
        }

        public override IEnumerator Play()
        {
            return this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1, null);
        }

        public override bool DoesHaveActivePlayMethod
        {
            get { return false; }
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            return this.GameController.ActiveTurnPhase.IsUsePower && this.GameController.ActiveTurnTaker == this.TurnTaker;
        }

        // Needs testing, but here's third power! 
        private bool isHeroUsingThirdPower(UsePowerAction upa)
        {
            return (upa.HeroUsingPower == this.DecisionMaker && this.Journal.UsePowerEntriesThisTurn().Count() == 2);
            //if (upa.HeroUsingPower != this.DecisionMaker) { return false; }
            //IEnumerable<UsePowerJournalEntry> powersThisTurn = this.Journal.UsePowerEntriesThisTurn();
            //if (powersThisTurn.Count() == 3 && powersThisTurn.ElementAt(2).PowerUser == this.TurnTaker)
            //{
            //    return true;
            //}
            //return false;
        }

        private IEnumerator increasePowerDamage(UsePowerAction upa)
        {
            this.tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.PowerSource == upa.Power, 2);
            this.storedUPA = upa;
            return null;
        }

        // Needs testing, but here's third power! 
        private bool isSamePower(UsePowerAction upa)
        {
            return (tempIncrease != null && upa == this.storedUPA);
        }

        private IEnumerator resetPowerDamage(UsePowerAction upa)
        {
            this.RemoveTrigger(tempIncrease);
            this.tempIncrease = null;
            this.storedUPA = null;
            return null;
        }

    }
}