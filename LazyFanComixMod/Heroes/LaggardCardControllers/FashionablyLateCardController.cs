using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Laggard
{
  public class FashionablyLateCardController : CardController
  {
    public FashionablyLateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Melee, 1, false, 0, cardSource: this.GetCardSource()), new TriggerType[] { TriggerType.DrawCard, TriggerType.UsePower });
    }

    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals =
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3)
            };


      // Deal <a> target <b> damage.
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());

    }

  }
}