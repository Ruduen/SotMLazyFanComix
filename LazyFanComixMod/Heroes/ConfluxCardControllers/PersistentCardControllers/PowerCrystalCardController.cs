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
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DealDamageOrBounceResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.MoveCard });
    }

    private IEnumerator DealDamageOrBounceResponse(PhaseChangeAction action)
    {
      IEnumerator coroutine;
      List<DealDamageAction> ddaResults = new List<DealDamageAction>();

      coroutine = this.DealDamage(this.CharacterCard, this.CharacterCard, 3, DamageType.Psychic, true, true, storedResults: ddaResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (!this.DidDealDamage(ddaResults, this.CharacterCard))
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