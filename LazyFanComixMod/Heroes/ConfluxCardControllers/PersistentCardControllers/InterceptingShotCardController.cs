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
      this.AddWhenDestroyedTrigger((DestroyCardAction dca) => this.GameController.SelectAndUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource()), TriggerType.DestroyCard);
    }

    private IEnumerator CounterDamageResponse(DealDamageAction action)
    {
      IEnumerator coroutine;

      coroutine = this.CancelAction(action);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (!action.IsPretend)
      {
        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }



  }
}



