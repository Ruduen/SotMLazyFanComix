using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.T210
{
  public class LoadoutKnifeCardController : LoadoutSharedCardController
  {
    public LoadoutKnifeCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator UsePower(int index = 0)
    {
      //bool isThirdPower = this.checkThirdPower();
      int[] powerNumerals = new int[]
      {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 2)
      };

      IEnumerator coroutine;
      DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);
      List<DealDamageAction> ddas;


      if (!this.isThirdPower)
      {
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.CharacterCard.AlternateTitleOrTitle + " will deal additional damage and may destroy an ongoing or environment cards.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        ddas = new List<DealDamageAction>()
            {
                new DealDamageAction(this.GetCardSource(), ds, null, powerNumerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), ds, null, powerNumerals[2], DamageType.Toxic)
            };
        coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(ddas, null, null, powerNumerals[0], powerNumerals[0]);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c != this.Card && (this.IsOngoing(c) || c.IsEnvironment), "ongoing or environment"), 1, false, 0, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      coroutine = PostPowerDestroy();
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}