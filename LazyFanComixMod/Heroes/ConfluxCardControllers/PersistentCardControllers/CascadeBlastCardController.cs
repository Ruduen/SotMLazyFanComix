using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class CascadeBlastCardController : ConfluxDamageTypesDealtCardController
  {
    public CascadeBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.HeroUsingPower == this.HeroTurnTakerController, DamageResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.DestroySelf }, TriggerTiming.After);
    }

    private IEnumerator DamageResponse(UsePowerAction action)
    {
      IEnumerator coroutine;

      List<DealDamageAction> ddaResults = new List<DealDamageAction>();

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), this.confluxDamageTypesDealt().Count() * 2, DamageType.Energy, 1, false, 0, storedResultsDamage: ddaResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


      if (ddaResults.Where((DealDamageAction dda) => dda.DidDealDamage && dda.Amount > 0).Count() > 0)
      {
        coroutine = this.GameController.SendMessageAction(this.Card.AlternateTitleOrTitle + " has dealt damage, so it will destroy itself.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

  }
}