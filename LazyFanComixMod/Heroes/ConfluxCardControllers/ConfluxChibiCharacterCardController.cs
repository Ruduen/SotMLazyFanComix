using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

namespace LazyFanComix.Conflux
{
  public class ConfluxChibiCharacterCardController : HeroCharacterCardController
  {
    public ConfluxChibiCharacterCardController(Card card, TurnTakerController turnTakerController)
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
      int damage;

      List<DiscardCardAction> dcaResults = new List<DiscardCardAction>(); int[] powerNumerals = new int[] {
        this.GetPowerNumeral(0, 1),
        this.GetPowerNumeral(1, 1),
        this.GetPowerNumeral(1, 2)
      };

      if(this.HeroTurnTakerController != null)
      {
        coroutine = this.DrawCards(this.HeroTurnTakerController, powerNumerals[0]);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        if(this.confluxDamageTypesDealt().Count() <= 0)
        {
          coroutine = this.GameController.SendMessageAction(this.Card?.AlternateTitleOrTitle + " has not dealt damage, so no cards can be discarded.", Priority.Low, this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        else
        {
          coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, confluxDamageTypesDealt().Count() * powerNumerals[1], false, 0, dcaResults, cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
      }
      else
      {
        coroutine = this.GameController.SendMessageAction(this.Card?.AlternateTitleOrTitle + " does not have a hand, so no cards can be drawn or discarded.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

        damage = this.GetNumberOfCardsDiscarded(dcaResults);
      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), damage, DamageType.Fire, powerNumerals[2], false, 0, cardSource: this.GetCardSource());
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
            coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => this.IsOngoing(c), "ongoing"), 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 2:
          {
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsEnvironmentTarget, "environment target"), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            Card envTarget = this.GetSelectedCard(scdResults);
            if (envTarget != null)
            {
              coroutine = this.GameController.DealDamage(this.DecisionMaker, envTarget, (Card c) => !c.IsEnvironment, 2, DamageType.Fire, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

              coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, envTarget), envTarget, 10, DamageType.Fire, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
      }
      yield break;
    }

  }
}