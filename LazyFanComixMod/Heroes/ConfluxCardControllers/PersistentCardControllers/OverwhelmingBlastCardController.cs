using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class OverwhelmingBlastCardController : CardController
  {
    public OverwhelmingBlastCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddMakeDamageIrreducibleTrigger((DealDamageAction dda) => dda.DamageSource.Card == this.CharacterCard);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;

      int[] powerNums = new int[]
      {
        this.GetPowerNumeral(0, 1),
        this.GetPowerNumeral(1, 3)
      };

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNums[1], DamageType.Energy, powerNums[0], false, powerNums[0], true, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

  }
}