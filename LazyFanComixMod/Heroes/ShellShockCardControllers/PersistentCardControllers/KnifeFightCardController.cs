using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class KnifeFightCardController : CardController
  {
    public KnifeFightCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == this.CharacterCard && dda.DamageType == DamageType.Melee, 1);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == this.CharacterCard && dda.DamageType == DamageType.Melee, (DealDamageAction dda) => 1);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNum = new int[]
      {
        this.GetPowerNumeral(0, 2),
        this.GetPowerNumeral(1, 2)
      };
      IEnumerator coroutine;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNum[1], DamageType.Melee, powerNum[0], false, 0, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}