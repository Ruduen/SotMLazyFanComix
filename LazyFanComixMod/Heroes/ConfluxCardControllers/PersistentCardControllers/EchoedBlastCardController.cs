using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class EchoedBlastCardController : CardController
  {
    private const string _FirstPowerUsedThisTurn = "FirstPowerUsedThisTurn";

    public EchoedBlastCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstPowerUsedThisTurn, this.CharacterCard.Title + " has already used a power this turn.", this.CharacterCard.Title + " has not yet used a power this turn.");
    }

    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.HeroUsingPower == this.HeroTurnTakerController && !this.IsPropertyTrue(_FirstPowerUsedThisTurn), TrackAndRepeatPowerResponse, TriggerType.UsePower, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstPowerUsedThisTurn), TriggerType.Hidden);
    }

    private IEnumerator TrackAndRepeatPowerResponse(UsePowerAction upa)
    {
      IEnumerator coroutine;

      this.SetCardPropertyToTrueIfRealAction(_FirstPowerUsedThisTurn);

      coroutine = this.GameController.UsePower(upa.Power, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}