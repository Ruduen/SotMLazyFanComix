using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Laggard
{
  public class StallTacticsCardController : CardController
  {

    public StallTacticsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      Card card = this.GetCardThisCardIsNextTo();
      this.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, card != null && !card.IsHeroCharacterCard);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == card, (DealDamageAction dda) => 1);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card == card, (DealDamageAction dda) => 1);
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => DiscardOrDestroy(), new TriggerType[] { TriggerType.DiscardCard, TriggerType.DestroySelf });
    }

    private IEnumerator DiscardOrDestroy()
    {
      IEnumerator coroutine;
      List<DiscardCardAction> dcaResult = new List<DiscardCardAction>();
      coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 2, false, 0, dcaResult, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (!this.DidDiscardCards(dcaResult, 2))
      {
        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
    {
      // Find what to go next to.
      IEnumerator coroutine = this.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget, "target"), storedResults, isPutIntoPlay, decisionSources);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}