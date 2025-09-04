using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

namespace LazyFanComix.Conflux
{
  public class ConfluxCharacterCardController : HeroCharacterCardController
  {
    public ConfluxCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowIfElseSpecialString(() => confluxDamageTypesDealt().Count() == 0, () => string.Format("{0} has dealt no damage this turn.", this.CharacterCard.AlternateTitleOrTitle), () => string.Format("{0} has dealt {1} different types of damage this turn: {2}", this.CharacterCard.AlternateTitleOrTitle, confluxDamageTypesDealt().Count(), this.damageTypeList()));
    }

    private DamageType[] confluxDamageTypesDealt()
    {
      return this.Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.Card && ddje.Amount > 0).Select((DealDamageJournalEntry ddje) => ddje.DamageType).Distinct().ToArray();
    }

    private string damageTypeList()
    {
      DamageType[] damageTypes = this.confluxDamageTypesDealt();
      string result = null;
      foreach (string damageType in damageTypes.Select((DamageType dt) => dt.ToString()))
      {
        if (string.IsNullOrEmpty(result))
        {
          result = damageType.ToString();
        }
        else
        {
          result = string.Format("{0}, {1}", result, damageType.ToString());
        }
      }
      return result;
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;
      DamageType dt;

      List<SelectDamageTypeDecision> sdtResults = new List<SelectDamageTypeDecision>();
      int[] powerNumerals = new int[] {
        this.GetPowerNumeral(0, 1),
        this.GetPowerNumeral(1, 2)
      };

      coroutine = this.GameController.SelectDamageType(this.HeroTurnTakerController, sdtResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      dt = sdtResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], dt, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
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
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.UntilStartOfNextTurn(this.TurnTaker);
            coroutine = this.AddStatusEffect(idse, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
      }
      yield break;
    }
  }
}