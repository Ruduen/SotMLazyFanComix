using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Laggard
{
  public class ChronicleOfUnknownScenesCardController : CardController
  {
    public ChronicleOfUnknownScenesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      return this.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: this.GetCardSource());
    }

    public override IEnumerator UsePower(int index = 0)
    {
      // TODO - Ported from Eyepiece - consider separate cleanup.
      List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
      IEnumerator coroutine;
      coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location l) => true, storedResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      Location deck = this.GetSelectedLocation(storedResults);
      List<Card> storedCards = new List<Card>();
      if (deck != null)
      {
        coroutine = this.RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(this.HeroTurnTakerController, this.TurnTakerController, deck, 1, 1, 1, storedCards);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      if (deck != null)
      {
        List<Location> list = new List<Location>();
        list.Add(deck.OwnerTurnTaker.Revealed);
        coroutine = this.GameController.CleanupCardsAtLocations(this.TurnTakerController, list, deck, false, true, false, false, false, true, storedCards, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      yield break;
    }
  }
}