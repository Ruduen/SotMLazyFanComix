using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class PowerCrystalCardController : ConfluxDamageTypesDealtCardController
  {
    public PowerCrystalCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }


    public override void AddTriggers()
    {
      // Damage type increase.
      this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.CharacterCard) && !this.IsHeroTarget(dd.Target), DamageIncreaseAmount);
      // Self damage.
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DiscardOrBounceResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.MoveCard });
    }

    private IEnumerator DiscardOrBounceResponse(PhaseChangeAction action)
    {
      IEnumerator coroutine;
      List<DiscardCardAction> dcaResults = new List<DiscardCardAction>();

      coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 2, false, 0, dcaResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (dcaResults.Where((dca) => dca.WasCardDiscarded).Count() != 2)
      {
        coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, this.Card, this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    private int DamageIncreaseAmount(DealDamageAction action)
    {
      return this.confluxDamageTypesDealt().Count();
    }

  }
}