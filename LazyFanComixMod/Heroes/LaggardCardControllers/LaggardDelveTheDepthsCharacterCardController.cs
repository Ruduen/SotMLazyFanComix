﻿using Handelabra.Sentinels.Engine;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace LazyFanComix.Laggard
{
  public class LaggardDelveTheDepthsCharacterCardController : HeroCharacterCardController
  {
    public string str;

    public LaggardDelveTheDepthsCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"));
    }

    public override IEnumerator UsePower(int index = 0)
    {
      return this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.HeroTurnTakerController, this.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain("hindsight"), "hindsight"), 1);
    }
    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location l) => l.IsDeck, storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            Location selectedLocation = this.GetSelectedLocation(storedResults);
            if (selectedLocation != null)
            {
              coroutine = this.GameController.RevealAndReplaceCards(this.HeroTurnTakerController, selectedLocation, 1, new List<Card>(), cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
              break;
            }
            yield break;
          }
        case 1:
          {
            coroutine = this.GameController.SelectHeroToMoveCard(this.HeroTurnTakerController, (HeroTurnTakerController h) => h.HeroTurnTaker.Hand, (HeroTurnTakerController h) => h.TurnTaker.Trash, optionalMoveCard: false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 2:
          {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.PlayTopCard, (Location l) => l.IsDeck && !l.IsEmpty, storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            Location selectedLocation = this.GetSelectedLocation(storedResults);
            if (selectedLocation != null)
            {
              coroutine = this.GameController.PlayTopCardOfLocation(this.FindTurnTakerController(selectedLocation.OwnerTurnTaker), selectedLocation, responsibleTurnTaker: this.HeroTurnTaker, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
      }
      yield break;
    }
  }
}