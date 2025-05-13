using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;


namespace LazyFanComix.Laggard
{
  public class SpiritualVisionCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => c.IsCharacter && c != this.CharacterCard, "another character"); } }

    public SpiritualVisionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => MoveAndMayPlayCard(), new TriggerType[] { TriggerType.MoveCard, TriggerType.PlayCard });
    }

    private IEnumerator MoveAndMayPlayCard()
    {
      // TODO: Improve logic to better handle OblivAeon cases! For now, only using cards from the owner's deck, to limit handling.
      IEnumerator coroutine;
      TurnTaker owner = this.GetCardThisCardIsNextTo().Owner;
      coroutine = this.GameController.SelectAndMoveCard(this.HeroTurnTakerController, (Card c) => c.NativeDeck == owner.Deck && c.IsInTrash, owner.Deck, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.PlayTopCardOfLocation(this.FindTurnTakerController(owner), owner.Deck, true, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}