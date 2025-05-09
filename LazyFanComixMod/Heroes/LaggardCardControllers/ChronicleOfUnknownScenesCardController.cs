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
      this.AddIncreaseDamageTrigger((DealDamageAction dda) => !dda.Target.IsHero && dda.Target.NextToLocation.Cards.Any((Card c) => c.DoKeywordsContain("hindsight")), amountToAdjust);
      this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target.IsHero && dda.Target.NextToLocation.Cards.Any((Card c) => c.DoKeywordsContain("hindsight")), amountToAdjust);
    }

    private int amountToAdjust(DealDamageAction dda)
    {
      return dda.Target.NextToLocation.Cards.Where((Card c) => c.DoKeywordsContain("hindsight")).Count();
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals =
        {
          this.GetPowerNumeral(0, 1),
          this.GetPowerNumeral(1, 1)
        };
      Function[] functions =
        {
          new Function(this.HeroTurnTakerController, "Play " + powerNumerals[1] + " hindsight Card", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, powerNumerals[1],false,powerNumerals[1],
                   new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"),
                  cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.HeroTurnTakerController) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.DoKeywordsContain("hindsight"))),
          new Function(this.HeroTurnTakerController, "Draw " + powerNumerals[0] + " Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, powerNumerals[0], cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController))
        };

      SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, functions, true, null, this.TurnTaker.Name + " cannot play any hindsight cards or draw any cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

      IEnumerator coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}