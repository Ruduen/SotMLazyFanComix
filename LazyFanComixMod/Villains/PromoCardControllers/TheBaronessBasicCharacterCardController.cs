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
      return card.Owner == base.TurnTaker && card.Identifier == "Vampirism";
    }


    public override void AddSideTriggers()
    {
      if (!this.Card.IsFlipped)
      {
        this.SideTriggers.Add(
          this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => StartOfTurnResponse(), new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.FlipCard })
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
      }
      this.AddDefeatedIfDestroyedTriggers(false);
      base.AddSideTriggers();
    }

    private IEnumerator StartOfTurnResponse()
    {
      IEnumerator coroutine;
      List<Card> played = new List<Card>();

      coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("scheme"), "scheme"), 1, storedPlayResults: played);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

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

      ITrigger tempIncrease = this.AddToTemporaryTriggerList(this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.Card == this.Card && GetSchemeCount() > 0, (DealDamageAction dda) => GetSchemeCount()));

      coroutine = this.DealDamageToLowestHP(this.Card, 1, (Card c) => this.IsHeroTarget(c), (Card c) => 1, DamageType.Infernal, numberOfTargets: this.H);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      this.RemoveTemporaryTrigger(tempIncrease);
    }

    private int GetSchemeCount()
    {
      return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("scheme")).Count();
    }

    private IEnumerator EoTFlipResponse()
    {
      IEnumerator coroutine;

      if(GetSchemeCount() == 0)
      {
        coroutine = this.GameController.SendMessageAction("No scheme cards are in play, so Baroness flips.", Priority.Medium, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}