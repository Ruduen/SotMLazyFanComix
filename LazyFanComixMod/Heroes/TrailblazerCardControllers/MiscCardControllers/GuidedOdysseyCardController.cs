using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public class GuidedOdysseyCardController : CardController
    {
        public GuidedOdysseyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Play a card from the environment trash.
            coroutine = this.GameController.SelectAndPlayCard(this.HeroTurnTakerController, (Card c) => c.Location == this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Trash, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play a position from your trash.
            coroutine = this.GameController.SelectAndPlayCard(this.HeroTurnTakerController, (Card c) => c.IsPosition && c.Location == this.HeroTurnTaker.Trash, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Up to two players may play the top card of their deck.
            coroutine = this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController,
                new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.Custom,
                (TurnTaker tt) => this.GameController.PlayTopCard(this.GameController.FindHeroTurnTakerController(tt.ToHero()), this.GameController.FindHeroTurnTakerController(tt.ToHero()), cardSource: this.GetCardSource()),
                2, false, 0, cardSource: this.GetCardSource()
             );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Select the player to play the top card of their deck", decision.HeroTurnTakerController.TurnTaker + "is selecting the player to play the top card of their deck", "Vote for the player to play the top card of their deck", "Player to play the top card of their deck");
        }
    }
}