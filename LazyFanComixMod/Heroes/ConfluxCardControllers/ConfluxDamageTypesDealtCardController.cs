using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public abstract class ConfluxDamageTypesDealtCardController : CardController
  {
    public ConfluxDamageTypesDealtCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowIfElseSpecialString(() => confluxDamageTypesDealt().Count() == 0, () => string.Format("{0} has dealt no damage this turn.", this.CharacterCard.AlternateTitleOrTitle), () => string.Format("{0} has dealt {1} different types of damage this turn: {2}", this.CharacterCard.AlternateTitleOrTitle, confluxDamageTypesDealt().Count(), this.damageTypeList()));
    }
    protected DamageType[] confluxDamageTypesDealt()
    {
      return this.Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.CharacterCard && ddje.Amount > 0).Select((DealDamageJournalEntry ddje) => ddje.DamageType).Distinct().ToArray();
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

  }
}