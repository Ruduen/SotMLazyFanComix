using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigurationFlashFireCardController : CardController
  {
    public ConfigurationFlashFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>((UsePowerAction upa)=>upa.HeroUsingPower == this.HeroTurnTakerController,DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
    }

    private IEnumerator DealDamageResponse(UsePowerAction action)
    {
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Fire, 1, false, 1, true, cardSource: this.GetCardSource());
    }
  }
}