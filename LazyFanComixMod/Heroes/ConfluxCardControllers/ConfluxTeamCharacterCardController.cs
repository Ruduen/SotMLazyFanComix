using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

namespace LazyFanComix.Conflux
{
  public class ConfluxTeamCharacterCardController : PromoDefaultCharacterCardController
  {
    public ConfluxTeamCharacterCardController(Card card, TurnTakerController turnTakerController)
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
      List<SelectCardDecision> scdResults;

      List<Card> selected = new List<Card>();
      int[] powerNumerals = new int[] {
        this.GetPowerNumeral(0, 1),
        this.GetPowerNumeral(1, 1),
        this.GetPowerNumeral(2, 1),
        this.GetPowerNumeral(3, 1)
      };

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Energy, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (confluxDamageTypesDealt().Count() * powerNumerals[2] == 0)
      {
        coroutine = this.GameController.SendMessageAction(this.Card?.AlternateTitleOrTitle + " has not dealt damage this turn, so no targets can be selected.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      for (int i = 0; i < confluxDamageTypesDealt().Count() * powerNumerals[2]; i++)
      {
        scdResults = new List<SelectCardDecision>();
        coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !selected.Contains(c), "targets in play"), scdResults, true, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        Card card = this.GetSelectedCard(scdResults);
        if (card == null)
        {
          break;
        }
        else
        {
          selected.Add(card);
          ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumerals[3]);
          rdse.TargetCriteria.IsSpecificCard = card;
          rdse.UntilStartOfNextTurn(this.TurnTaker);
          rdse.UntilCardLeavesPlay(card);
          coroutine = this.AddStatusEffect(rdse, true);
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
      }
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
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.ReduceDamageTaken, new LinqCardCriteria((Card c) => !c.IsIncapacitatedOrOutOfGame && this.IsHeroCharacterCard(c), "hero character"), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectCardDecision scd = scdResults.FirstOrDefault<SelectCardDecision>();
            if (scd != null && scd.SelectedCard != null)
            {
              ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
              rdse.TargetCriteria.IsSpecificCard = scd.SelectedCard;
              rdse.UntilStartOfNextTurn(this.TurnTaker);
              coroutine = this.AddStatusEffect(rdse, true);
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
        case 2:
          {
            DamageType dt;
            List<SelectDamageTypeDecision> sdtResults = new List<SelectDamageTypeDecision>();

            coroutine = this.GameController.SelectDamageType(this.HeroTurnTakerController, sdtResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            dt = sdtResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;

            ChangeDamageTypeStatusEffect cdtse = new ChangeDamageTypeStatusEffect(dt);
            cdtse.UntilStartOfNextTurn(this.TurnTaker);
            cdtse.BeforeOrAfter = BeforeOrAfter.Before;
            coroutine = this.AddStatusEffect(cdtse, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
      }
      yield break;
    }

  }
}