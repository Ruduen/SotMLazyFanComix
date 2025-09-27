using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;

namespace LazyFanComix.ShellShock
{
  public class ZapLadCharacterCardController : PromoDefaultCharacterCardController
  {
    public ZapLadCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;
      SelectCardsDecision scd;
      List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 5)
            };
      if (this.DecisionMaker.IsHero)
      {
        coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      scd = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.DoKeywordsContain("device"), SelectionType.GainHP, 1, false, 0, true, cardSource: this.GetCardSource());

      coroutine = this.GameController.SelectCardsAndDoAction(scd, (SelectCardDecision d) => this.GameController.GainHPEx(d.SelectedCard, powerNumerals[1], allowExceedMaxHP: true, cardSource: this.GetCardSource()));
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    public override IEnumerator PerformEnteringGameResponse()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.MoveCards(this.DecisionMaker, new Card[] {this.FindCard("ZapLad")}, this.TurnTaker.OutOfGame, false, true, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.TurnTaker.IsHero && this.HeroTurnTaker.NumberOfCardsInHand < 4 && this.TurnTaker.Deck.NumberOfCards > 0)
      {
        coroutine = this.GameController.MoveCards(this.DecisionMaker, this.TurnTaker.Deck.GetTopCards(4 - this.HeroTurnTaker.NumberOfCardsInHand), this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    // TODO: Replace Incap with something more unique!
  }
}