using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Laggard
{
  public class GlimpseOfTheUnknownCardController : CardController
  {
    public GlimpseOfTheUnknownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddWhenDestroyedTrigger((DestroyCardAction dca) => this.GameController.SelectAndUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource()), TriggerType.UsePower);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals = {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };

      IEnumerator coroutine;

      coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment || this.GameController.IsOngoing(c), "environment or ongoing"), powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsHero && !c.IsCharacter, "hero non-character"), powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}