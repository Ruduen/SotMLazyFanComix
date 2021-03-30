using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Inquirer
{
    // TODO: TEST!
    public class UntilYouMakeItCardController : CardController
    {
        public UntilYouMakeItCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Search for Persona.
            coroutine = this.SearchForCards(this.DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("persona"), () => "persona"), true, false, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw card.
            coroutine = this.DrawCard(null, true, null, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play card.
            coroutine = this.SelectAndPlayCardFromHand(this.DecisionMaker, true, null, null, false, false, true, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}