using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class EyeFromAboveCardController : CardController
  {
    private const string _FirstEnterPlayThisTurn = "FirstEnterPlayThisTurn";

    public EyeFromAboveCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstEnterPlayThisTurn, "A non-hero target has entered play this turn.", "A non-hero target has not yet entered play this turn.");
    }

    public override void AddTriggers()
    {
      this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => !this.IsPropertyTrue(_FirstEnterPlayThisTurn) && !cepa.CardEnteringPlay.IsHero && cepa.CardEnteringPlay.IsTarget, UsePowerResponse, TriggerType.UsePower, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstEnterPlayThisTurn), TriggerType.Hidden);
    }

    private IEnumerator UsePowerResponse(CardEntersPlayAction action)
    {
      IEnumerator coroutine;

      this.SetCardPropertyToTrueIfRealAction(_FirstEnterPlayThisTurn);

      coroutine = this.GameController.SelectAndUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

  }
}