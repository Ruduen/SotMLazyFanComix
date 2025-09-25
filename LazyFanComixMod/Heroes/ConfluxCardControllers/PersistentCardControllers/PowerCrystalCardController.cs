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
    }

    private int DamageIncreaseAmount(DealDamageAction action)
    {
      return this.confluxDamageTypesDealt().Count() / 2;
    }

  }
}