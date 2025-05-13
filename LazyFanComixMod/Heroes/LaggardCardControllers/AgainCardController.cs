using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Laggard
{
  public class AgainCardController : CardController
  {
    public AgainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectCardFromLocationAndMoveIt(this.HeroTurnTakerController, this.HeroTurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"), new MoveCardDestination(this.HeroTurnTaker.PlayArea).ToEnumerable(), cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.ShuffleTrashIntoDeck(this.HeroTurnTakerController, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}