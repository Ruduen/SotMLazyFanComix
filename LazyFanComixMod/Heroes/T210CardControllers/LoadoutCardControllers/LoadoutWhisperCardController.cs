using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.T210
{
  public class LoadoutWhisperCardController : LoadoutSharedCardController
  {
    public LoadoutWhisperCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator UsePower(int index = 0)
    {
      //bool isThirdPower = this.checkThirdPower();
      int[] powerNumerals = new int[]
      {
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
      };
      DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);

      IEnumerator coroutine;
      Func<DealDamageAction, IEnumerator> statusEffect = null;

      if (this.isThirdPower)
      {
        coroutine = this.GameController.SendMessageAction("This is the third power, so damage will be reduced until the start of your next turn and " + this.CharacterCard.AlternateTitleOrTitle + " will draw " + powerNumerals[3] + " cards.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        statusEffect = (DealDamageAction dd) => this.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd, powerNumerals[2]);
      }

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, ds, powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, 0, true, addStatusEffect: statusEffect, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.isThirdPower)
      {
        coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[3], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }


    }
  }
}