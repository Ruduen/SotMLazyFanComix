using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AkashThriya
{
    public class AkashThriyaPlantEssenceCharacterCardController : PromoDefaultCharacterCardController
    {
        public AkashThriyaPlantEssenceCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            Location envDeck = this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Deck;

            coroutine = this.GameController.PlayTopCardOfLocation(this.DecisionMaker, envDeck, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker,
                new List<Function>(){
                    new Function(this.DecisionMaker, "Destroy an Environment Card", SelectionType.DestroyCard, () => this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), 1, cardSource: this.GetCardSource()), this.FindCardsWhere((Card c) => c.IsInPlay && c.IsEnvironment).Count() > 0, this.TurnTaker.Name + " cannot shuffle the top Card of their Deck into the Environment Deck, so they must Destroy an Environment Card."),
                    new Function(this.DecisionMaker, "Shuffle the top card of your deck into the Environment deck", SelectionType.ShuffleCardIntoDeck, () => this.GameController.ShuffleCardIntoLocation(this.DecisionMaker, this.HeroTurnTaker.Deck.TopCard, envDeck, false, cardSource: this.GetCardSource()), this.HeroTurnTaker.Deck.HasCards, this.TurnTaker.Name + " cannot Destroy an Environment Card, so they must Shuffle the top Card of their Deck into the Environment Deck.")
                }, 
                false, null, this.TurnTaker.Name + " cannot draw or play any cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}