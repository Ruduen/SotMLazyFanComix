using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigAutoAssaultCardController : CardController
  {

    public ConfigAutoAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyPlayResponse, TriggerType.PlayCard);
    }

    private IEnumerator DestroyPlayResponse(PhaseChangeAction action)
    {
      IEnumerator coroutine;
      List<DestroyCardAction> dca = new List<DestroyCardAction>();

      coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && !c.IsCharacter && c != this.Card, "other non-character"), 1, false, 0, storedResultsAction: dca, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (dca.Any((DestroyCardAction dca) => dca.WasCardDestroyed))
      {
        coroutine = this.GameController.SendMessageAction("A card was destroyed by " + this.Card.Title + ", so you may play card.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        coroutine = this.GameController.SendMessageAction("No card was destroyed by " + this.Card.Title + ", so you cannot play an extra card using it.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

  }
}