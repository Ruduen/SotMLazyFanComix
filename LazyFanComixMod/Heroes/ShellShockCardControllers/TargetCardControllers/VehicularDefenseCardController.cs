using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class VehicularDefenseCardController : CardController
  {
    public VehicularDefenseCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target.Owner == this.TurnTaker && dda.Target.DoKeywordsContain("vehicle") && dda.DamageSource != null && dda.DamageSource.IsTarget && !dda.DamageSource.IsHero && dda.DidDealDamage, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
    }

    private IEnumerator DealDamageResponse(DealDamageAction action)
    {
      return this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => c == action.DamageSource.Card, 2, DamageType.Melee, cardSource: this.GetCardSource());
    }

    public override IEnumerator UsePower(int index = 0)
    {
      return this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, new LinqCardCriteria((Card c) => this.GameController.DoesCardContainKeyword(c, "vehicle"), "vehicle"), cardSource: this.GetCardSource());
    }
  }
}