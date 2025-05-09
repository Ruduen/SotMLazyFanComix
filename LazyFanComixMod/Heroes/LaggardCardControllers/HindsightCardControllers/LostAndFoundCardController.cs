using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Laggard;
using System.Collections;


namespace LazyFanComix.Laggard
{
  public class LostAndFoundCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => c.IsTarget, "target"); } }

    public LostAndFoundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => this.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, (Card c) => c == this.GetCardThisCardIsNextTo(), 6, DamageType.Projectile, cardSource: this.GetCardSource()), new TriggerType[] { TriggerType.DealDamage });
    }

  }
}