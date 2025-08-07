using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.T210
{
  public abstract class LoadoutSharedCardController : CardController
  {
    private bool _isThirdPower;
    private UsePowerAction _thirdPowerUpa;

    public bool isThirdPower
    {
      get { return _isThirdPower; }
    }

    public LoadoutSharedCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
      _isThirdPower = false;
      _thirdPowerUpa = null;
    }

    public override IEnumerator Play()
    {
      return this.GameController.UsePower(this.Card, 0, cardSource: this.GetCardSource());
    }

    // Use trigger on power use to note that something is the third power use, since that has better potential to handle ordering given timing point.
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

    protected IEnumerator PostPowerDestroy()
    {
      IEnumerator coroutine;
      int destroyedCount;
      List<DestroyCardAction> dcaResults = new List<DestroyCardAction>();

      coroutine = this.GameController.DestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("loadout") && c.Owner == this.TurnTaker && c != this.Card, "other loadout"), storedResults: dcaResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); }
      else { this.GameController.ExhaustCoroutine(coroutine); }

      destroyedCount = dcaResults.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Count();
      for (int i = 0; i < destroyedCount; i++)
      {
        coroutine = this.GameController.UsePower(this.Card, 0, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); }
        else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

  }
}