using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Laggard
{
  public class StallTacticsCardController : CardController
  {

    public StallTacticsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == this.CharacterCard, (DealDamageAction dda) => 1);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card == this.CharacterCard, (DealDamageAction dda) => 1);
      this.AddStartOfTurnTrigger(delegate (TurnTaker tt)
      {
        if (this.TurnTaker == tt)
        {
          return this.CharacterCard.HitPoints.GetValueOrDefault() < this.CharacterCard.MaximumHitPoints.GetValueOrDefault() & (this.CharacterCard.HitPoints != null & this.CharacterCard.MaximumHitPoints != null);
        }
        return false;
      }, (PhaseChangeAction p) => RegainAndDestroy(), new TriggerType[] { TriggerType.GainHP, TriggerType.DestroySelf }, null, false);
    }

    private IEnumerator RegainAndDestroy()
    {
      IEnumerator coroutine;
      List<GainHPAction> gha = new List<GainHPAction>();

      YesNoAmountDecision yesNo = new YesNoAmountDecision(this.GameController, this.DecisionMaker, SelectionType.GainHP, 3, false, false, null, null, this.GetCardSource());
      coroutine = this.GameController.MakeDecisionAction(yesNo, true);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.DidPlayerAnswerYes(yesNo))
      {
        coroutine = this.GameController.GainHP(this.CharacterCard, 3, storedResults: gha, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      if (gha.Count() > 0 && gha.First().AmountActuallyGained > 0)
      {
        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}