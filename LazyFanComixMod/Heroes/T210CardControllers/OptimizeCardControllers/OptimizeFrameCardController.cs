using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.T210
{
  public class OptimizeFrameCardController : OptimizeSharedCardController
  {
    public OptimizeFrameCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override LinqCardCriteria AppropriateCards()
    {
      return new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c == this.CharacterCard);
    }

    protected override IEnumerator MatchCardAction()
    {
      return this.GameController.GainHP(this.CharacterCard, 4, cardSource: this.GetCardSource());
    }

  }
}