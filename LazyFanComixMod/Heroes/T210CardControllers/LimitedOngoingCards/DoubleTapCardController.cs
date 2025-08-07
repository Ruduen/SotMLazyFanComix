using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.T210
{
  public class DoubleTapCardController : CardController
  {
    public DoubleTapCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      base.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
    }

    public override void AddTriggers()
    {
      this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine = this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1, null);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }

    public override bool DoesHaveActivePlayMethod
    {
      get { return false; }
    }

    private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
    {
      return tt == this.TurnTaker;
    }

    public override bool AskIfIncreasingCurrentPhaseActionCount()
    {
      return this.GameController.ActiveTurnPhase.IsUsePower && this.ShouldIncreasePhaseActionCount(this.GameController.ActiveTurnTaker);
    }
  }
}