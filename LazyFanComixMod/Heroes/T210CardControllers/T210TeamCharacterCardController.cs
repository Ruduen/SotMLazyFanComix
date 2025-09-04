using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

// Manually tested!

namespace LazyFanComix.T210
{
  public class T210TeamCharacterCardController : HeroCharacterCardController
  {
    private bool _isThirdPower;
    private UsePowerAction _thirdPowerUpa;

    public T210TeamCharacterCardController(Card card, TurnTakerController turnTakerController)
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
      int[] powerNumerals;
      IEnumerator coroutine;

      if (index == 1)
      {
        powerNumerals = new int[]
        {
          this.GetPowerNumeral(0, 2),
          this.GetPowerNumeral(1, 1)
        };
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        powerNumerals = new int[]
        {
          this.GetPowerNumeral(0, 1),
          this.GetPowerNumeral(1, 2)
        };
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }


    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            coroutine = this.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 1:
          {
            List<SelectLocationDecision> sldResults = new List<SelectLocationDecision>();
            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.DiscardFromDeck, (Location l) => true, sldResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (sldResults.Count > 0 && sldResults?.First()?.SelectedLocation.Location != null)
            {
              coroutine = this.GameController.DiscardTopCards(this.HeroTurnTakerController, sldResults.First().SelectedLocation.Location, 1, responsibleTurnTaker: this.TurnTaker, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
        case 2:
          {
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            List<DealDamageAction> ddaResults = new List<DealDamageAction>();
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.HeroToDealDamage, new LinqCardCriteria((Card c) => !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndNotUnderCard && this.IsHeroCharacterCard(c), "Hero character"), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectCardDecision scd = scdResults.FirstOrDefault();
            if (scd?.SelectedCard != null)
            {
              coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, scd.SelectedCard), 1, DamageType.Projectile, 1, false, 1, storedResultsDamage: ddaResults, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            if (ddaResults.Count > 0 && ddaResults.First().DidDealDamage == false)
            {
              coroutine = this.EachPlayerUsesAPower(this.HeroTurnTakerController);
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
      }
      yield break;
    }
  }
}