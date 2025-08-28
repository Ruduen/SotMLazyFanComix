using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class InterceptingShotCardController : CardController
  {
    public InterceptingShotCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == this.CharacterCard && dda.DamageSource?.IsHeroTarget == false, CounterDamageResponse, new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.CancelAction, TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.Before, orderMatters: true);
    }

    private IEnumerator CounterDamageResponse(DealDamageAction action)
    {
      IEnumerator coroutine;

      coroutine = this.CancelAction(action);

      if (!action.IsPretend)
      {
        // Trigger to increase damage dealt by damage dealt.
        ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, action.Amount);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        // Deal damage.
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        this.RemoveTrigger(tempIncrease);

        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }



  }
}



