using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Guise
{
    public class GuiseShenanigansCharacterCardController : PromoDefaultCharacterCardController
    {
        public GuiseShenanigansCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[]{
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            list.Add(new Function(this.DecisionMaker, "Play " + numerals[1] + " ongoing and make it indestructible", SelectionType.PlayCard, () => PlayOngoingAndMakeIndestructable(numerals[1]), this.CanPlayCardsFromHand(this.DecisionMaker) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsOngoing), this.TurnTaker.Name + " cannot draw any cards, so they must play " + numerals[1] + " ongoing and make it indestructible."));
            list.Add(new Function(this.DecisionMaker, "Draw " + numerals[0] + " Card", SelectionType.DrawCard, () => this.DrawCards(this.DecisionMaker, numerals[0]), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any ongoings, so they must draw " + numerals[0] + " card."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw any cards or play any ongoings, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


        }

        private IEnumerator PlayOngoingAndMakeIndestructable(int numeral)
        {
            List<PlayCardAction> pcaResults = new List<PlayCardAction>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, numeral, false, cardCriteria: new LinqCardCriteria((Card c) => c.IsOngoing), storedResults: pcaResults, cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<Card> playedAndValidCards = pcaResults.Where((PlayCardAction pca) => pca.WasCardPlayed && pca.CardToPlay.IsInPlay).Select((PlayCardAction pca) => pca.CardToPlay).ToList();

            if (playedAndValidCards.Count > 0)
            {
                MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
                makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsOneOfTheseCards = playedAndValidCards;
                makeIndestructibleStatusEffect.CardsToMakeIndestructible.OwnedBy = this.HeroTurnTaker;
                makeIndestructibleStatusEffect.UntilEndOfNextTurn(this.HeroTurnTaker);
                coroutine = this.AddStatusEffect(makeIndestructibleStatusEffect, true);
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                string turnTakerName;

                // Set up response.
                if (base.TurnTaker.IsHero)
                {
                    turnTakerName = this.TurnTaker.Name;
                }
                else
                {
                    turnTakerName = this.Card.Title;
                }
                coroutine = this.GameController.SendMessageAction(turnTakerName + " does not have any valid Ongoings in play, so he cannot make any indestructible. Whoops!", Priority.Medium, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}