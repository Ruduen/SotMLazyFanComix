using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace LazyFanComix.Laggard
{
  public class ChronicleOfUnknownScenesCardController : CardController
  {
    public ChronicleOfUnknownScenesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddIncreaseDamageTrigger((DealDamageAction dda) => !dda.Target.IsHero && dda.Target.NextToLocation.Cards.Any((Card c) => c.DoKeywordsContain("hindsight")), 1);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target.IsHero && dda.Target.NextToLocation.Cards.Any((Card c) => c.DoKeywordsContain("hindsight")), (DealDamageAction dda) => 1);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals =
        {
          this.GetPowerNumeral(0, 1),
        };
      IEnumerator coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, powerNumerals[0], false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"), cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}