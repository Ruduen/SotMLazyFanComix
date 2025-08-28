using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class EyeOfTheVortexCardController : CardController
  {
    public EyeOfTheVortexCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(DestroyCardResponse), TriggerType.DestroyCard);
    }

    private IEnumerator DestroyCardResponse(DestroyCardAction action)
    {
      return this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment || this.IsOngoing(c), "ongoing or environment"), 3, false, 0, cardSource: this.GetCardSource());
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

  }
}