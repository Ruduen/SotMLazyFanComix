using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Laggard
{
  public class LateToThePartyCardController : CardController
  {
    public LateToThePartyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 4, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 3, false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"), cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.ImmediatelyEndTurn(this.HeroTurnTakerController, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}