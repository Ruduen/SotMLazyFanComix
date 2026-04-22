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
      this.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => this.GameController.DoesCardContainKeyword(c, "device"), "device"));
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstDamageUsedThisTurn, "One of " + this.CharacterCard.Title + "'s cards has already dealt damage this turn.", "One of " + this.CharacterCard.Title + "'s cards has not yet already dealt damage this turn.");
    }

    public override void AddTriggers()
    {
      this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == this.CharacterCard && this.FindCardsWhere(new LinqCardCriteria((Card c) => this.GameController.DoesCardContainKeyword(c, "device") && c.IsInPlayAndHasGameText, "device")).Count() > 0, 1);
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Owner == this.TurnTaker && !dda.Target.IsHero && !this.IsPropertyTrue(_FirstDamageUsedThisTurn), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstDamageUsedThisTurn), TriggerType.Hidden);
    }

    private IEnumerator DealDamageResponse(DealDamageAction action)
    {
      IEnumerator coroutine;

      this.SetCardPropertyToTrueIfRealAction(_FirstDamageUsedThisTurn);

      if (action.Target.IsInPlayAndHasGameText)
      {
        coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => c == action.Target, 1, DamageType.Melee, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }
  }
}