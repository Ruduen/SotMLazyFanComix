using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.T210
{
  public class OptimizeAwarenessCardController : OptimizeSharedCardController
  {
    public OptimizeAwarenessCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override LinqCardCriteria AppropriateCards()
    {
      return new LinqCardCriteria((Card c) => IsOngoing(c));
    }

    protected override IEnumerator MatchCardAction()
    {
      return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.DrawCard, (TurnTaker tt) => this.GameController.DrawCard(tt.ToHero(), true, cardSource: this.GetCardSource()), 3, false, 0, cardSource: this.GetCardSource());
    }

  }
}