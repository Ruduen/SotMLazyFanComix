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

      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => DrawOrPower(), new TriggerType[] { TriggerType.DrawCard, TriggerType.UsePower });
    }

    private IEnumerator DrawOrPower()
    {
      Function[] functions =
        {
        new Function(this.HeroTurnTakerController, "Use a Power", SelectionType.UsePower, () => this.GameController.SelectAndUsePower(this.HeroTurnTakerController, false, cardSource: this.GetCardSource()), this.GameController.CanUsePowers(this.HeroTurnTakerController, cardSource: this.GetCardSource()), this.TurnTaker.Name + " cannot draw any cards, so they must use a power."),
        new Function(this.HeroTurnTakerController, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot use any powers, so they must draw a card.")
        };

      SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, functions, false, null, this.TurnTaker.Name + " cannot use any powers or draw any cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

      IEnumerator coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
    public override IEnumerator UsePower(int index = 0)
    {
      int[] powerNumerals =
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3)
            };


      // Deal <a> target <b> damage.
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], true, cardSource: this.GetCardSource());

    }

  }
}