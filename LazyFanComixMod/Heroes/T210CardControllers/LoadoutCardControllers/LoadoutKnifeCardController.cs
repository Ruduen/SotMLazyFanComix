using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 2),
      };

      IEnumerator coroutine;
      List<DealDamageAction> ddas = new List<DealDamageAction>();
      DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Toxic, powerNumerals[0], false, 0, storedResultsDamage: ddas, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.isThirdPower)
      {
        coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.CharacterCard.AlternateTitleOrTitle + " may destroy an ongoing or environment card or target with 5 or fewer HP.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c != this.Card && (this.IsOngoing(c) || c.IsEnvironment || (c.IsTarget && c.HitPoints <= 5)), "ongoing, environment, or target with 5 HP", false, false, "ongoing card, environment card, or target with 5 HP", "ongoing cards, environment cards, or targets with 5 HP"), 1, false, 0, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}