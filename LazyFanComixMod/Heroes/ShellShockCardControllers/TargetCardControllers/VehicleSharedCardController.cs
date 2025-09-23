using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public abstract class VehicleSharedCardController : CardController
  {
    public VehicleSharedCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddDealDamageAtStartOfTurnTrigger(this.TurnTaker, this.Card, (Card c) => c == this.Card, TargetType.All, 3, DamageType.Lightning);
    }

    public override IEnumerator Play()
    {
      return this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && c.DoKeywordsContain("vehicle") && c != this.Card, "vehicle"), cardSource: this.GetCardSource());
    }
  }
}