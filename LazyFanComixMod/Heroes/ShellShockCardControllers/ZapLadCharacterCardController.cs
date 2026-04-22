using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;

namespace LazyFanComix.ShellShock
{
  public class ZapLadCharacterCardController : HeroCharacterCardController
  {
    public ZapLadCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;
      SelectCardsDecision scd;
      ReduceDamageStatusEffect rdse;
      List<GainHPAction> gha = new List<GainHPAction>();
      List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 1)
            };
      if (this.DecisionMaker.IsHero)
      {
        coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      coroutine = this.GameController.SelectAndGainHP(this.DecisionMaker, powerNumerals[1], false, (Card c) => this.GameController.DoesCardContainKeyword(c, "device"), 1, 0, storedResults: gha, cardSource: this.GetCardSource()); ;
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (gha.Where((gha) => gha.HpGainer != null).Any())
      {
        rdse = new ReduceDamageStatusEffect(powerNumerals[2]);
        rdse.TargetCriteria.IsSpecificCard = gha.FirstOrDefault().HpGainer;
        rdse.UntilEndOfNextTurn(this.TurnTaker);
        coroutine = this.GameController.AddStatusEffect(rdse, true, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    public override IEnumerator PerformEnteringGameResponse()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.MoveCards(this.DecisionMaker, new Card[] { this.FindCard("ZapLad") }, this.TurnTaker.OutOfGame, false, true, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.TurnTaker.IsHero && this.HeroTurnTaker.NumberOfCardsInHand < 4 && this.TurnTaker.Deck.NumberOfCards > 0)
      {
        coroutine = this.GameController.MoveCards(this.DecisionMaker, this.TurnTaker.Deck.GetTopCards(4 - this.HeroTurnTaker.NumberOfCardsInHand), this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }


    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
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
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardToHandFromTrash, new LinqCardCriteria((Card c) => c.IsInTrash && c.Location.IsHero && this.IsEquipment(c), "equipment cards in any hero's trash", false), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResults.Any((scd) => scd.Completed && scd.SelectedCard != null))
            {
              Card card = scdResults.FirstOrDefault().SelectedCard;
              coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, card, card.Location.OwnerTurnTaker.ToHero().Hand, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
        case 2:
          {
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsTarget && this.GameController.DoesCardContainKeyword(c, "device") && c.IsInPlayAndHasGameText, "device target"), scdResults, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResults.Any((scd) => scd.Completed && scd.SelectedCard != null))
            {
              Card card = scdResults.FirstOrDefault().SelectedCard;
              coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, card), 5, DamageType.Lightning, 1, false, 1, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
      }
      yield break;
    }
  }
}