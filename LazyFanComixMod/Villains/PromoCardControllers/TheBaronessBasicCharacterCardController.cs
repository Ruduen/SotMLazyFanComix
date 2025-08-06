using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheBaroness
{
  public class TheBaronessBasicCharacterCardController : VillainCharacterCardController
  {
    public TheBaronessBasicCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override bool AskIfCardIsIndestructible(Card card)
    {
      return card.Owner == this.TurnTaker && card.Identifier == "Vampirism";
    }


    public override void AddSideTriggers()
    {
      if (!this.Card.IsFlipped)
      {
        int revealmax = this.IsGameAdvanced ? 2 : 0;
        this.SideTriggers.Add(
          this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => StartOfTurnResponse(revealmax), new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.FlipCard })
          );
      }
      else
      {
        this.SideTriggers.Add(
          this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => EoTDamageResponse(), new TriggerType[] { TriggerType.DealDamage, TriggerType.IncreaseDamage })
          );
        this.SideTriggers.Add(
          this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => EoTFlipResponse(), new TriggerType[] { TriggerType.FlipCard })
          );
        if (this.IsGameAdvanced)
        {
          this.AddReduceDamageTrigger((Card c) => c == this.Card, 1);
        }
      }
      this.AddDefeatedIfDestroyedTriggers(false);
      base.AddSideTriggers();
      if (this.IsGameChallenge)
      {
        this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card == this.Card && GetSchemeCount() > 0, (DealDamageAction dda) => GetSchemeCount());
      }
    }

    private IEnumerator StartOfTurnResponse(int revealMax)
    {
      IEnumerator coroutine;
      List<Card> played = new List<Card>();

      if (this.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("scheme"))).Count() <= revealMax)
      {
        coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("scheme"), "scheme"), 1, storedPlayResults: played);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      if (played?.Count == 0)
      {
        coroutine = this.GameController.SendMessageAction("No scheme was put into play, so Baroness flips.", Priority.Medium, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }
    private IEnumerator EoTDamageResponse()
    {
      IEnumerator coroutine;
      coroutine = this.DealDamageToLowestHP(this.Card, 1, (Card c) => this.IsHeroTarget(c), (Card c) => 2, DamageType.Infernal, numberOfTargets: this.H);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    private int GetSchemeCount()
    {
      return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("scheme")).Count();
    }

    private IEnumerator EoTFlipResponse()
    {
      IEnumerator coroutine;

      if (GetSchemeCount() == 0)
      {
        coroutine = this.GameController.SendMessageAction("No scheme cards are in play, so the villain trash is shuffled into the villain deck and Baroness flips.", Priority.Medium, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.ShuffleTrashIntoDeck(this.TurnTakerController, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}