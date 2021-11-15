using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public abstract class TrailblazerPositionCardController : CardController
    {
        public TrailblazerPositionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Destroy all other positions.
            // Should there be a position check? It's probably fine, but the official source has one.
            coroutine = this.GameController.DestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsPosition && c != this.Card, "position"), false, selectionType: SelectionType.DestroyCard, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}