using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

// Manually tested!

namespace LazyFanComix.T210
{
  public class T210ChibiCharacterCardController : PromoDefaultCharacterCardController
  {
    private bool _isFifthPower;
    private UsePowerAction _fifthPowerUpa;

    public T210ChibiCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
      _isFifthPower = false;
      _fifthPowerUpa = null;
    }
    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>(checkIsFifthPower, setIsFifthPower, TriggerType.HiddenLast, TriggerTiming.Before);
      this.AddTrigger<UsePowerAction>(checkWasFifthPower, clearIsFifthPower, TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false);
    }

    private bool checkIsFifthPower(UsePowerAction upa)
    {
      return (this == upa.Power.CardController && this.Journal.UsePowerEntriesThisTurn().Count() == 4);
    }

    private IEnumerator setIsFifthPower(UsePowerAction upa)
    {
      this._isFifthPower = true;
      this._fifthPowerUpa = upa;
      yield break;
    }

    private bool checkWasFifthPower(UsePowerAction upa)
    {
      return (this._isFifthPower && this._fifthPowerUpa == upa);
    }

    private IEnumerator clearIsFifthPower(UsePowerAction upa)
    {
      this._isFifthPower = false;
      this._fifthPowerUpa = null;
      yield break;
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals = new int[]
      {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(1, 9)
      };

      // Deal <a> target <b> damage.
      IEnumerator coroutine;

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      // Check if this is the Fifth power.
      if (this._isFifthPower)
      {
        coroutine = this.GameController.SendMessageAction("This is the Fifth power, so " + this.Card.AlternateTitleOrTitle + " deals " + powerNumerals[1] + " target " + powerNumerals[2] + "additional damage.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[2], DamageType.Projectile, powerNumerals[1], false, powerNumerals[1], true, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }


    }

  }
}