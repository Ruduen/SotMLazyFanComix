using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class InTheWeedsCardController : CardController
  {
    private const string _FirstDamageUsedThisTurn = "FirstDamageUsedThisTurn";

    public InTheWeedsCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("device"), "device"));
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstDamageUsedThisTurn, "One of " + this.CharacterCard.Title + "'s cards has already dealt damage this turn.", "One of " + this.CharacterCard.Title + "'s cards has not yet already dealt damage this turn.");
    }

    public override void AddTriggers()
    {
      this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == this.CharacterCard && this.FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("device") && c.IsInPlayAndHasGameText, "device")).Count() > 0, 1);
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Owner == this.TurnTaker && !this.IsPropertyTrue(_FirstDamageUsedThisTurn), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstDamageUsedThisTurn), TriggerType.Hidden);
    }

    private IEnumerator DealDamageResponse(DealDamageAction action)
    {
      IEnumerator coroutine;

      this.SetCardPropertyToTrueIfRealAction(_FirstDamageUsedThisTurn);

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Lightning, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}