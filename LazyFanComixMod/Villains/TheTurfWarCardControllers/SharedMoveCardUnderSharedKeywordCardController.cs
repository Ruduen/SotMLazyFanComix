using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public abstract class SharedMoveCardUnderSharedKeywordCardController : CardController
    {
        public SharedMoveCardUnderSharedKeywordCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected IEnumerator MoveCardUnderFigureheadWithSharedKeyword(Card card)
        {
            return MoveCardsUnderFigureheadWithSharedKeyword(new List<Card>() { card });
        }
        protected IEnumerator MoveCardsUnderFigureheadWithSharedKeyword(IEnumerable<Card> cards)
        {
            IEnumerator coroutine;
            if(cards.Count() == 0)
            {
                coroutine = this.GameController.SendMessageAction("There are no cards to move.", Priority.Low, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                IEnumerable<Card> validCharacters = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("figurehead") && c.DoKeywordsContain(this.Card.GetKeywords()));
                if (validCharacters.Count() > 0)
                {
                    SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCardToUnderCard, validCharacters, cardSource: this.GetCardSource());
                    coroutine = this.GameController.SelectCardAndDoAction(scd, (SelectCardDecision scd)=> MoveCardsUnderSelectedDestination(scd, cards));
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    coroutine = this.GameController.SendMessageAction("There is no Figurehead that shares a keyword with " + this.Card.Title + ".", Priority.Medium, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }

        protected IEnumerator MoveCardsUnderSelectedDestination(SelectCardDecision scd, IEnumerable<Card> cardsToMove)
        {
            IEnumerator coroutine;
            if (scd?.SelectedCard?.UnderLocation != null)
            {
                coroutine = this.GameController.MoveCards(this.TurnTakerController, cardsToMove, scd.SelectedCard.UnderLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}
