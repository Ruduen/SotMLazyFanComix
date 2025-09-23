using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;


namespace LazyFanComix.T210
{
  public class T210CharacterCardController : HeroCharacterCardController
  {
    private bool _isThirdPower;
    private UsePowerAction _thirdPowerUpa;

    public T210CharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
      _isThirdPower = false;
      _thirdPowerUpa = null;
    }
    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>(checkIsThirdPower, setIsThirdPower, TriggerType.HiddenLast, TriggerTiming.Before);
      this.AddTrigger<UsePowerAction>(checkWasThirdPower, clearIsThirdPower, TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false);
    }

    private bool checkIsThirdPower(UsePowerAction upa)
    {
      return (this == upa.Power.CardController && this.Journal.UsePowerEntriesThisTurn().Count() == 2);
    }

    private IEnumerator setIsThirdPower(UsePowerAction upa)
    {
      this._isThirdPower = true;
      this._thirdPowerUpa = upa;
      yield break;
    }

    private bool checkWasThirdPower(UsePowerAction upa)
    {
      return (this._isThirdPower && this._thirdPowerUpa == upa);
    }

    private IEnumerator clearIsThirdPower(UsePowerAction upa)
    {
      this._isThirdPower = false;
      this._thirdPowerUpa = null;
      yield break;
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals = new int[]
      {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(1, 3)
      };

      // Deal <a> target <b> damage.
      IEnumerator coroutine;

      // Trigger to increase damage by 3 if appropriate.
      ITrigger tempIncrease = null;

      // Check if this is the third power.
      if (this._isThirdPower)
      {
        coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.Card.AlternateTitleOrTitle + " deals " + powerNumerals[2] + " additional damage.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, powerNumerals[2]);
      }

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (tempIncrease != null)
      {
        this.RemoveTrigger(tempIncrease);
      }
    }

    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            coroutine = this.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 1:
          {
            coroutine = this.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 2:
          {
            List<DiscardCardAction> dcaResults = new List<DiscardCardAction>();
            coroutine = this.GameController.SelectHeroToDiscardCards(this.HeroTurnTakerController, 0, 3, storedResultsDiscard: dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dcaResults.Count() > 0 && dcaResults?.First()?.HeroTurnTakerController != null)
            {
              coroutine = this.GameController.SelectTargetsAndDealDamage(dcaResults.First().HeroTurnTakerController, new DamageSource(this.GameController, dcaResults.First().HeroTurnTakerController.CharacterCard), 3, DamageType.Projectile, this.GetNumberOfCardsDiscarded(dcaResults), false, 0, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            }
            break;
          }
      }
      yield break;
    }

    //public override bool ShouldChangeCutout(CutoutInfo currentInfo, GameAction action, ActionTiming timing, out CutoutInfo changedInfo, out CutoutAnimation animation)
    //{
    //  bool cutoutChangeFlag = base.ShouldChangeCutout(currentInfo, action, timing, out changedInfo, out animation);
    //  if (this.TurnTakerControllerWithoutReplacements != null && this.TurnTakerController.IsHero && action != null && action is UsePowerAction && timing == ActionTiming.DidPerform)
    //  {
    //    UsePowerAction upa = action as UsePowerAction;
    //    if (checkIsThirdPower(upa))
    //    {
    //      List<CutoutInfo> cutoutList = new List<CutoutInfo>()
    //      {
    //        new CutoutInfo
    //        {
    //          Identifier = "ThirdEffects",
    //          IsEffect = true,
    //          EffectDuration = 1f
    //        }
    //      };
    //      changedInfo.ExtraCutouts = cutoutList;
    //      cutoutChangeFlag = true;
    //    }
    //    else
    //    {
    //      // TODO: Check if fourth?
    //      cutoutChangeFlag = true;
    //    }
    //  }

    //  return cutoutChangeFlag;
    //}

  }
}