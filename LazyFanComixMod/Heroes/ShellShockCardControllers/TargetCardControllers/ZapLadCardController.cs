using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class ZapLadCardController : CardController
  {
    public ZapLadCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddPreventDamageTrigger((DealDamageAction dd) => dd.Target.DoKeywordsContain("vehicle") && dd.Target.Owner == this.TurnTaker && dd.DamageType == DamageType.Lightning, (DealDamageAction dda) => this.GameController.GainHP(dda.Target, 1, cardSource: this.GetCardSource()), new TriggerType[] { TriggerType.GainHP }, true);
      this.AddDealDamageAtEndOfTurnTrigger(this.TurnTaker, this.Card, (Card c) => true, TargetType.SelectTarget, 1, DamageType.Lightning);
    }

  }
}