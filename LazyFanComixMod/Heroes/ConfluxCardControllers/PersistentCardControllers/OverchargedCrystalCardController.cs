using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class OverchargedCrystalCardController : CardController
  {
    public OverchargedCrystalCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;

      int[] powerNumerals = new int[]
      {
             this.GetPowerNumeral(0, 2),
             this.GetPowerNumeral(1, 2),
             this.GetPowerNumeral(2, 2),
             this.GetPowerNumeral(3, 1)
      };

      coroutine = this.GameController.GainHP(this.HeroTurnTakerController, (Card c) => this.IsHeroTarget(c), powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, (Card c) => !this.IsHeroTarget(c), powerNumerals[1], DamageType.Energy, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, powerNumerals[2], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && !c.IsCharacter, "your non-character"), powerNumerals[3], false, powerNumerals[3], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}