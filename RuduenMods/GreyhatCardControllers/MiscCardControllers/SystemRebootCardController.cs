using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Greyhat
{
    public class SystemRebootCardController : CardController
    {
        public SystemRebootCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<MoveCardAction> mcaResults = new List<MoveCardAction>();
            IEnumerator coroutine;

            // Search for and add a link.
            coroutine = this.SearchForCards(this.DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.IsLink, "link"), false, true, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Bounce links.
            coroutine = this.GameController.SelectAndReturnCards(this.DecisionMaker, null, new LinqCardCriteria((Card c) => c.IsLink, "link"), true, false, false, 0, mcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            int numCardsMoved = this.GetNumberOfCardsMoved(mcaResults);
            if (numCardsMoved > 0)
            {
                coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, numCardsMoved, false, 0, new LinqCardCriteria((Card c) => c.IsLink, "link"), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("No link cards were returned to " + this.TurnTaker.Name + "'s hand, so no cards will be played.", Priority.Medium, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}