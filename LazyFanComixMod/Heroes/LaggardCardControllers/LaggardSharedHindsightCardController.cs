using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Laggard
{
  public abstract class LaggardSharedHindsightCardController : CardController
  {
    protected abstract LinqCardCriteria NextToCriteria { get; }

    public LaggardSharedHindsightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      // For "next to" cards - adjust location.
      this.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false, this.GetCardThisCardIsNextTo() != null && !GetCardThisCardIsNextTo().IsHeroCharacterCard);
      // Unique triggers.
      this.AddUniqueTriggers();
      // Selfdestruct triggers. Occur after the others.
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource()), new TriggerType[] { TriggerType.DestroySelf });
    }

    public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
    {
      // Find what to go next to.
      IEnumerator coroutine = this.SelectCardThisCardWillMoveNextTo(this.NextToCriteria, storedResults, isPutIntoPlay, decisionSources);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }


    protected abstract void AddUniqueTriggers();

  }
}