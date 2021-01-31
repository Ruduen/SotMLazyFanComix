using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
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

            // Play a position from your trash. 
            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.IsPosition && c.Location == this.HeroTurnTaker.Trash, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play a card from the environment trash. 
            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.GameController.FindEnvironmentTurnTakerController().TurnTaker.Trash, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Up to two players may play the top card of their deck. 
            coroutine = this.GameController.SelectTurnTakersAndDoAction(this.DecisionMaker,
                new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.PlayTopCard,
                (TurnTaker tt) => this.GameController.PlayTopCard(this.GameController.FindHeroTurnTakerController(tt.ToHero()), this.GameController.FindHeroTurnTakerController(tt.ToHero()), cardSource: this.GetCardSource()),
                2, false, 0, cardSource: this.GetCardSource()
             );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}