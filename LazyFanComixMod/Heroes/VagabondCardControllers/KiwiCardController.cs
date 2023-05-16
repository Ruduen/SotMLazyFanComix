using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class KiwiCardController : CardController
    {
        public KiwiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimePowerUsed, "{0} has already taken effect this turn.", "{0} has not yet taken effect this turn.").Condition = (() => base.Card.IsInPlayAndHasGameText);
        }

        private const string FirstTimePowerUsed = "FirstTimePowerUsed";

        public override void AddTriggers()
        {
            this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa?.Power?.TurnTakerController == this.DecisionMaker && upa?.Power?.CardSource?.Card?.IsInPlayAndHasGameText == true && !this.IsPropertyTrue(FirstTimePowerUsed, null), RepeatPowerResponse, TriggerType.UsePower, TriggerTiming.After, ActionDescription.Unspecified, respondEvenIfPlayedAfterAction: true);
            //this.AddTrigger<UsePowerAction>(IsFirstPowerUsed, RepeatPowerResponse, TriggerType.UsePower, TriggerTiming.After, ActionDescription.Unspecified, respondEvenIfPlayedAfterAction: true);
            this.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay(FirstTimePowerUsed), TriggerType.Hidden);
        }

        //private bool IsFirstPowerUsed(UsePowerAction upa)
        //{
        //    //Quit if different hero.
        //    if (!(upa?.Power?.TurnTakerController == this.DecisionMaker))
        //    {
        //        return false;
        //    }

        //    // Get first power use by this hero.
        //    UsePowerJournalEntry upje = this.GameController.Game.Journal.UsePowerEntriesThisTurn().FirstOrDefault((UsePowerJournalEntry upje) => upje.PowerUser == this.HeroTurnTaker);
        //    if (upje == null)
        //    {
        //        return false;
        //    }

        //    // Quit if not the first used.Due to wording, use journal, so sequenced powers don't break! (AKA playing Boots into a second power.)
        //    // Match power based on both card granting power and power index.
        //    if (upje.CardWithPower != upa.Power.CardController.CardWithoutReplacements || upje.PowerIndex != upa.Power.Index || upje.CardSource != upa.Power.CardSource?.Card || upje.CardThatAllowedPowerToBeUsed != upa.CardSource?.Card)
        //    {
        //        return false;
        //    }

        //    // Otherwise looks good, move forward.
        //    return true;
        //}

        private IEnumerator RepeatPowerResponse(UsePowerAction upa)
        {
            List<YesNoCardDecision> yncdResults = new List<YesNoCardDecision>();

            Power power = null;
            IEnumerator coroutine;

            //// Sanity check for contributed powers. Creating a new instance of the power does not seem to work, so instead, manually retrieve it.
            //// Check based on same power sources and contribution, since situation could've changed. Most annoying example: Plague rat losing powers due to order of destruction.
            //if (upa.Power.IsContributionFromCardSource)
            //{
            //    IEnumerable<Power> powers = upa.Power.CardSource.CardController.AskIfContributesPowersToCardController(upa.Power.HeroCharacterCardUsingPower);
            //    if (powers != null)
            //    {
            //        power = powers.FirstOrDefault((Power p) => p.Description == upa.Power.Description);
            //        if (power == null)
            //        {
            //            // If this is the case of a contributed power that is no longer available, which is possible if, say, plague rat's second power is used to destroy an infection, then quit since the question is no longer relevant.
            //            yield break;
            //        }
            //    }
            //}

            if (upa.Power.IsContributionFromCardSource)
            {
                IEnumerable<Power> powers = this.GameController.GetAllPowersForCardController(upa.Power.CardController);
                power = powers.FirstOrDefault((Power p) => p.CardSource.Card == upa.Power.CardSource.Card && p.Title == upa.Power.Title && p.Description == upa.Power.Description);
                if (power == null)
                {
                    yield break;
                }
            }

            coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.UsePowerAgain, upa.Power.CardSource.Card, null, yncdResults, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yncdResults))
            {
                this.SetCardPropertyToTrueIfRealAction(FirstTimePowerUsed, null, null);
                if (power != null)
                {
                    coroutine = this.GameController.UsePower(power, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    //// Create a new instance of the power. This isn't perfect, but it's best to not rely on name/index checking which can be fuzzy, especially if the power is no longer granted, such as an infection being destroyed and causing the powre to be lost. EDIT: Doesn't work.
                    //Power power = new Power(upa.Power.TurnTakerController.ToHero(), upa.Power.CardController, upa.Power.Description, upa.Power.Method, upa.Power.Index, upa.Power.CopiedFromCardController, upa.Power.CardSource, upa.Power.HeroCharacterCardUsingPower);
                    //if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.UsePower(upa.Power.CardController.Card, upa.Power.Index, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}