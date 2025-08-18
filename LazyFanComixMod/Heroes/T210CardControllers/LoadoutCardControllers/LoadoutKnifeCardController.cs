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
      IEnumerable<Card> damaged;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Toxic, powerNumerals[0], false, 0, storedResultsDamage: ddas, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.isThirdPower)
      {
        coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.CharacterCard.AlternateTitleOrTitle + " may destroy a non-character, ongoing, or environment card.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        damaged = ddas.Where((DealDamageAction dda) => dda.DidDealDamage == true && !dda.Target.IsCharacter).Select((DealDamageAction dda) => dda.Target);

        coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c != this.Card && (this.IsOngoing(c) || c.IsEnvironment || damaged.Contains(c)), "non-character, ongoing, or environment"), 1, false, 0, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}