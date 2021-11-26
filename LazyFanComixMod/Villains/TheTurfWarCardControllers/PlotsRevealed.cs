using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class PlotsRevealedCardController : CardController
    {
        public PlotsRevealedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<Card> playedCards = new List<Card>();
            IEnumerator coroutine;
            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.IsTarget, "target"), 1, storedPlayResults: playedCards, shuffleReturnedCards: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if(playedCards != null && playedCards.Count() > 0)
            {
                coroutine = this.GameController.DealDamage(this.DecisionMaker, playedCards.FirstOrDefault(), (Card c) => !c.DoKeywordsContain(playedCards.FirstOrDefault().GetKeywords()), 2, DamageType.Toxic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}