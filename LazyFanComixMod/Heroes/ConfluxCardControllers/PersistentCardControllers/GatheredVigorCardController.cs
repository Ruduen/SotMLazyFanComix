using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Conflux;

namespace LazyFanComix.Conflux
{
  public class GatheredVigorCardController : CardController
  {
    public GatheredVigorCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.AddAsPowerContributor();
    }

    public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
    {
      if (cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && cardController.Card.Owner.IsPlayer && !cardController.Card.Owner.IsIncapacitatedOrOutOfGame && !cardController.Card.IsFlipped)
      {
        Power power = new Power(cardController.HeroTurnTakerController, cardController, "This hero deals themselves 2 psychic damage. Reveal cards from the top of your deck until a Limited card is revealed. You may put this card either into your hand or into play. Shuffle the rest of the revealed cards your deck.", () => SearchResponse(cardController), 0, null, this.GetCardSource());
        return new Power[]
        {
          power
        };
      }
      return null;
    }

    private IEnumerator SearchResponse(CardController characterCard)
    {
      IEnumerator coroutine;
      HeroTurnTakerController httc;
      MoveCardDestination[] destinations;
      List<Card> revealedCards;
      List<RevealCardsAction> rcaResults = new List<RevealCardsAction>();
      int[] powerNums = new int[]
      {
        this.GetPowerNumeral(0, 2)
      };

      if (characterCard?.TurnTaker?.IsPlayer == true)
      {
        // If this is a player, hand is a valid destination.
        httc = this.FindHeroTurnTakerController(characterCard.TurnTaker.ToHero());
        destinations = new MoveCardDestination[]
        {
          new MoveCardDestination(httc.TurnTaker.PlayArea),
          new MoveCardDestination(httc.HeroTurnTaker.Hand)
        };
      }
      else
      {
        // Otherwise, play only. 
        httc = this.DecisionMaker;
        destinations = new MoveCardDestination[]
        {
          new MoveCardDestination(this.TurnTaker.PlayArea)
        };
      }

      coroutine = this.GameController.DealDamage(httc, characterCard.CharacterCard, (Card c) => c == characterCard.CharacterCard, 2, DamageType.Psychic, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.RevealCards(httc, httc.TurnTaker.Deck, (Card c) => c.IsLimited, 1, rcaResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      revealedCards = rcaResults.SelectMany((RevealCardsAction rc) => rc.RevealedCards).ToList();

      coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(httc, httc.TurnTaker.Revealed, 1, 1, new LinqCardCriteria((Card c) => c.IsLimited &&
      revealedCards.Contains(c), "limited"), destinations, true, true, selectionType: SelectionType.PutIntoHandOrPlay, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.CleanupCardsAtLocations(httc, new List<Location>() { httc.TurnTaker.Revealed }, httc.TurnTaker.Deck, shuffleAfterwards: true, cardsInList: revealedCards, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}