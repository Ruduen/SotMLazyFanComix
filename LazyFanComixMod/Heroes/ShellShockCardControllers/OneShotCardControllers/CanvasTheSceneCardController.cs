using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class CanvasTheSceneCardController : CardController
  {
    public CanvasTheSceneCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      Location trash;
      List<SelectLocationDecision> sldResults = new List<SelectLocationDecision>();
      int powerNum = this.GetPowerNumeral(0, 2);
      Card zapLad = this.FindCard("ZapLad");

      coroutine = this.GameController.DiscardTopCardsOfDecks(this.DecisionMaker, (Location l) => !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (zapLad?.IsInPlayAndHasGameText == true)
      {
        coroutine = this.GameController.SelectATrash(this.DecisionMaker, SelectionType.MoveCard, (Location l) => l.HasCards, sldResults, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        if (sldResults.Count() > 0)
        {
          trash = this.GetSelectedLocation(sldResults);
          coroutine = this.GameController.MoveCard(this.DecisionMaker, trash.TopCard, trash.OwnerTurnTaker.Deck, cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

      }

      if (!zapLad.IsInPlayAndHasGameText)
      {
        if (zapLad?.Location == this.TurnTaker.Deck || zapLad?.Location == this.TurnTaker.Trash)
        {
          coroutine = this.GameController.PlayCard(this.DecisionMaker, zapLad, true, cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        else
        {
          coroutine = this.GameController.SendMessageAction(zapLad.Title + " is not in your hand or deck. Where is he?", Priority.Low, this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        coroutine = this.GameController.ShuffleTrashIntoDeck(this.DecisionMaker, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

  }
}