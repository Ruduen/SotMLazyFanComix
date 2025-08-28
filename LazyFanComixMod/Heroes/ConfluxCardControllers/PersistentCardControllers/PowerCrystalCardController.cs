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
      this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.CharacterCard), DamageIncreaseAmount);
      // Self damage.
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.DealDamageOrDestroyThisCardResponse(pca, this.CharacterCard, this.CharacterCard, 1, DamageType.Psychic, true), new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
    }

    private int DamageIncreaseAmount(DealDamageAction action)
    {
      return this.confluxDamageTypesDealt().Count();
    }
  }
}