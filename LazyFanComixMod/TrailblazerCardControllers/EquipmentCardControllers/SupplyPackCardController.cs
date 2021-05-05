using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public class SupplyPackCardController : CardController
    {
        private const string _FirstPositionPlayedThisTurn = "FirstPositionPlayedThisTurn";

        public SupplyPackCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstPositionPlayedThisTurn, "A Position has already been played this turn.", "A Position has not yet been played this turn.");
        }

        public override void AddTriggers()
        {
            this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => !this.IsPropertyTrue(_FirstPositionPlayedThisTurn) && cepa.CardEnteringPlay.IsPosition && cepa.CardEnteringPlay.Owner == this.HeroTurnTaker, ResponseAction, TriggerType.UsePower, TriggerTiming.After);
            this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstPositionPlayedThisTurn), TriggerType.Hidden);
        }

        protected IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            this.SetCardPropertyToTrueIfRealAction(_FirstPositionPlayedThisTurn);

            // All of the relevant ownership means the positions should only have 1 power, so we can skip more complex handling.
            if (cepa.CardEnteringPlay.HasPowers)
            {
                IEnumerator coroutine = this.GameController.UsePower(cepa.CardEnteringPlay, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}