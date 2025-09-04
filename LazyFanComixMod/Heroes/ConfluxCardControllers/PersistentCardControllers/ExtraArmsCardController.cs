using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class ExtraArmsCardController : CardController
  {
    public ExtraArmsCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      base.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
    }

    public override void AddTriggers()
    {
      this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine = this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1, null);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
    public override bool DoesHaveActivePlayMethod
    {
      get { return false; }
    }

    private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
    {
      return tt == this.TurnTaker;
    }

    public override bool AskIfIncreasingCurrentPhaseActionCount()
    {
      return this.GameController.ActiveTurnPhase.IsUsePower && this.ShouldIncreasePhaseActionCount(this.GameController.ActiveTurnTaker);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals;
      IEnumerator coroutine;

      switch (index)
      {
        case 1:

          powerNumerals = new int[]
          {
             this.GetPowerNumeral(0, 1),
             this.GetPowerNumeral(1, 1)
          };
          coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

          break;
        case 2:

          powerNumerals = new int[]
          {
             this.GetPowerNumeral(0, 1),
             this.GetPowerNumeral(1, 1)
          };

          coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, powerNumerals[0], cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

          coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, powerNumerals[1], false, powerNumerals[1], cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


          break;
        default:

          powerNumerals = new int[]
          {
             this.GetPowerNumeral(0, 1),
             this.GetPowerNumeral(1, 1)
          };
          coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

          break;
      }
    }
  }
}