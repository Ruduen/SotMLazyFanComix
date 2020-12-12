using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.BreachMage
{
    // Manually tested!
    public class FocusCharmCardController : CardController
    {
        public FocusCharmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Select an appropriate breach and focus it.
            IEnumerator coroutine = this.GameController.SelectCardAndDoAction(
                new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.RemoveTokens, this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.FindTokenPool("FocusPool") != null && c.FindTokenPool("FocusPool").CurrentValue > 0)),
                (SelectCardDecision d) => this.GameController.RemoveTokensFromPool(d.SelectedCard.FindTokenPool("FocusPool"), 1, cardSource: this.GetCardSource()),
            false);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}