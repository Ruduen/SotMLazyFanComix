using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Charade
{
    public class CharadeRecklessPlansCharacterCardController : PromoDefaultCharacterCardController
    {
        public CharadeRecklessPlansCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowListOfCardsAtLocation(this.Card.UnderLocation, new LinqCardCriteria(), null);
        }

        public override IEnumerator UsePower(int index = 0)
        {

            int[] powerNums = new int[] { 
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 1)
            };
            IEnumerator coroutine;

            if (this.TurnTaker.Deck.Cards.Any())
            {
                coroutine = this.GameController.MoveCards(this.DecisionMaker, this.TurnTaker.Deck, this.CharacterCard.UnderLocation, powerNums[0], playIfMovingToPlayArea: false, responsibleTurnTaker: this.TurnTaker, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, this.CharacterCard.UnderLocation, powerNums[1], powerNums[1], new LinqCardCriteria(), new MoveCardDestination[] { new MoveCardDestination(this.HeroTurnTaker.Hand) }, cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectAndMoveCard(this.DecisionMaker, (Card c) => c.Location == this.CharacterCard.UnderLocation, this.HeroTurnTaker.Hand, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}