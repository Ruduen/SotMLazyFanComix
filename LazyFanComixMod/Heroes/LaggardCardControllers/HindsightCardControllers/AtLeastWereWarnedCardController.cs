using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace LazyFanComix.Laggard
{
  public class AtLeastWereWarnedCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c != this.CharacterCard, "another Hero"); } }

    public AtLeastWereWarnedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker && this.GetCardThisCardIsNextTo().IsHeroCharacterCard, (PhaseChangeAction p) => this.GameController.SelectAndPlayCardFromHand(this.GameController.FindHeroTurnTakerController(this.GetCardThisCardIsNextTo().Owner.ToHero()), true, cardSource: this.GetCardSource()), TriggerType.PlayCard);
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker && this.GetCardThisCardIsNextTo().IsHeroCharacterCard, (PhaseChangeAction p) => this.GameController.SelectAndUsePower(this.GameController.FindHeroTurnTakerController(this.GetCardThisCardIsNextTo().Owner.ToHero()), cardSource: this.GetCardSource()), TriggerType.UsePower);
    }

  }
}