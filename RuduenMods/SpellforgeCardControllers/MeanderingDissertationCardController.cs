using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Spellforge
{
    // TODO: TEST!
    public class MeanderingDissertationCardController : CardController
    {
        public MeanderingDissertationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine;
            int numberOfCardsDiscarded;
            coroutine = this.SelectAndDiscardCards(this.DecisionMaker, null, false, 0, storedResults, selectionType: SelectionType.DiscardCard);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            numberOfCardsDiscarded = this.GetNumberOfCardsDiscarded(storedResults);
            if (numberOfCardsDiscarded > 0)
            {
                coroutine = this.GameController.SelectHeroToDrawCards(this.DecisionMaker, numberOfCardsDiscarded, false, false, null, false, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes"), null, null, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " did not discard any cards, so no cards will be drawn.", Priority.High, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // You may play card.
            coroutine = this.SelectAndPlayCardFromHand(this.DecisionMaker, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}