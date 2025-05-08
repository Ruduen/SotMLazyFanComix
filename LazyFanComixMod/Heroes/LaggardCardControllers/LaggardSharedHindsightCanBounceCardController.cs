using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Laggard
{
  public abstract class LaggardSharedHindsightCanBounceCardController : LaggardSharedHindsightCardController
  {

    public LaggardSharedHindsightCanBounceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      base.AddTriggers();

      // All of the triggers necessary to bounce.
      this.AddTrigger<FlipCardAction>((FlipCardAction f) => f.CardToFlip.Card == this.GetCardThisCardIsNextTo() && this.GetCardThisCardIsNextTo().IsFaceDownNonCharacter, (FlipCardAction f) => this.UsePowerAndBounce(), new TriggerType[] { TriggerType.UsePower, TriggerType.MoveCard }, TriggerTiming.After);
      this.AddTrigger<BulkMoveCardsAction>((BulkMoveCardsAction b) => b.CardsToMove.Any((Card c) => c == this.GetCardThisCardIsNextTo() && b.Destination.IsInPlayAndNotUnderCard), (BulkMoveCardsAction b) => this.UsePowerAndBounce(), new TriggerType[] { TriggerType.UsePower, TriggerType.MoveCard }, TriggerTiming.After);
      this.AddTrigger<MoveCardAction>((MoveCardAction m) => m.CardToMove == this.GetCardThisCardIsNextTo() && m.Destination.IsInPlayAndNotUnderCard, (MoveCardAction m) => this.UsePowerAndBounce(), new TriggerType[] { TriggerType.UsePower, TriggerType.MoveCard }, TriggerTiming.After);
    }

    private IEnumerator UsePowerAndBounce()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectAndUsePower(this.HeroTurnTakerController, false, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, this.Card, this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    protected override bool KeepTriggers => true;

  }
}