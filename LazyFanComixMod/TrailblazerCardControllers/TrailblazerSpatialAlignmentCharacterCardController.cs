using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Trailblazer
{
    public class TrailblazerSpatialAlignmentCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public TrailblazerSpatialAlignmentCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();

            // Make a Position Indestructible until the end of the turn.
            if (this.FindCardsWhere((Card c) => c.IsPosition && c.IsInPlayAndHasGameText).Count() > 0)
            {
                // Select a position.
                coroutine = this.GameController.SelectCardsAndStoreResults(this.HeroTurnTakerController, SelectionType.MakeIndestructible,
                    (Card c) => c.IsPosition && c.IsInPlayAndHasGameText, 1,
                    storedResults, false, 1, cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                List<Card> selectedCards = this.GetSelectedCards(storedResults).ToList();
                if (selectedCards != null && selectedCards.Count() > 0)
                {
                    MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
                    makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsOneOfTheseCards = selectedCards;
                    makeIndestructibleStatusEffect.CardsToMakeIndestructible.OwnedBy = this.HeroTurnTaker;
                    makeIndestructibleStatusEffect.UntilThisTurnIsOver(this.Game);
                    coroutine = this.AddStatusEffect(makeIndestructibleStatusEffect, true);
                    if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                string turnTakerName;
                // Set up response.
                if (this.TurnTaker.IsHero)
                {
                    turnTakerName = this.TurnTaker.Name;
                }
                else
                {
                    turnTakerName = this.Card.Title;
                }
                coroutine = this.GameController.SendMessageAction("There are no Positions in play, so " + turnTakerName + " cannot make any indestructible.", Priority.Medium, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // You may play a card.
            coroutine = this.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace Incap with something more unique!
    }
}