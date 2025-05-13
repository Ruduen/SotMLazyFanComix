using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Laggard
{
  public class LateToThePartyCardController : CardController
  {
    public LateToThePartyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      List<Function> list = new List<Function>();
      SelectFunctionDecision sfd;
      IEnumerator coroutine;

      list.Add(new Function(this.HeroTurnTakerController, "Play up to 3 hindsight cards", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 3, false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"), cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.HeroTurnTakerController) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.DoKeywordsContain("hindsight"))));
      list.Add(new Function(this.HeroTurnTakerController, "Draw 5 cards", SelectionType.DiscardCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, 5, cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController) || this.HeroTurnTaker.HasCardsInHand));
      sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, true, null, this.TurnTaker.Name + " cannot draw any cards or play any hindsight cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

      coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.ImmediatelyEndTurn(this.HeroTurnTakerController, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}