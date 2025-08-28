using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class ChannelEssenceCardController : ConfluxDamageTypesDealtCardController
  {
    public ChannelEssenceCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DrawCardByCount, new TriggerType[] { TriggerType.DrawCard, TriggerType.DestroySelf });
    }

    private IEnumerator DrawCardByCount(PhaseChangeAction action)
    {
      IEnumerator coroutine;

      List<DrawCardAction> dca = new List<DrawCardAction>();

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, this.confluxDamageTypesDealt().Count(), false, storedResults: dca, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (dca?.Where((DrawCardAction dca) => dca.DidDrawCard).Count() >= 3)
      {
        coroutine = this.GameController.SendMessageAction(this.Card.AlternateTitleOrTitle + " has drawn 3 or more cards, so it will destroy itself.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }

    public override IEnumerator Play()
    {
      return this.GameController.GainHP(this.CharacterCard, 3, cardSource: this.GetCardSource());

    }
  }
}