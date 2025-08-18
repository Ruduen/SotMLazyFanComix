using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigFlashFlareCardController : CardController
  {
    public ConfigFlashFlareCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} powers have been used this turn.", this.Journal.UsePowerEntriesThisTurn().Count().ToString()));
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
    }

    private IEnumerator DealDamageResponse(PhaseChangeAction action)
    {
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), NumPowersUsedDynamic, DamageType.Fire, () => 1, false, 0, cardSource: this.GetCardSource());
    }

    private int? NumPowersUsedDynamic(Card card)
    {
      return this.Journal.UsePowerEntriesThisTurn().Count();
    }
  }
}