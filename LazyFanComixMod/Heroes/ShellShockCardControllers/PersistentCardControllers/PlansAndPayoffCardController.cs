using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class PlansAndPayoffCardController : CardController
  {
    public PlansAndPayoffCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay.DoKeywordsContain("vehicle") && cepa.CardEnteringPlay.IsTarget, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
      this.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(OnDestroyResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
    }

    private IEnumerator OnDestroyResponse(DestroyCardAction action)
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsOngoing(c), "ongoing"), 1, false, 0, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
        {
          new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.CharacterCard), null, 2, DamageType.Melee),
          new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.CharacterCard),null, 2, DamageType.Lightning)
        },
      null, null, 1, 1);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    private IEnumerator DealDamageResponse(CardEntersPlayAction action)
    {
      return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, action.CardEnteringPlay), 2, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
    }

    public override IEnumerator UsePower(int index = 0)
    {
      return this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: null);
    }
  }
}