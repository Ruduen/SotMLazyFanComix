using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.T210
{
  public abstract class OptimizeSharedCardController : CardController
  {
    public OptimizeSharedCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      return PlayHelper();
    }

    protected IEnumerator PlayHelper()
    {
      IEnumerable<Card> validCards;
      IEnumerator coroutine;
      List<UsePowerDecision> usePowerDecisions = new List<UsePowerDecision>();

      coroutine = this.GameController.SelectAndUsePower(this.HeroTurnTakerController, false, storedResults: usePowerDecisions, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      // If any UPD cards are in the selected cards...
      validCards = this.GameController.FindCardsWhere(AppropriateCards());
      if (usePowerDecisions.Where((UsePowerDecision upd) => validCards.Contains(upd.SelectedCard)).Any())
      {
        coroutine = MatchCardAction();
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }

    protected abstract LinqCardCriteria AppropriateCards();

    protected abstract IEnumerator MatchCardAction();

  }
}