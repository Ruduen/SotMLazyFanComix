using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public class LeadTheWayCardController : CardController
    {
        public LeadTheWayCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Search for position.
            coroutine = this.SearchForCards(this.HeroTurnTakerController, true, false, 1, 1, new LinqCardCriteria((Card c) => c.IsPosition, "position"), false, true, false, shuffleAfterwards: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw card.
            coroutine = this.GameController.DrawCard(this.HeroTurnTaker, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play card.
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}