using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.BreachMage
{
    public class FocusBreachCardController : CardController
    {
        public FocusBreachCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

            // Draw a card.
            coroutine = this.DrawCard(this.HeroTurnTaker, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select an appropriate breach and focus it.
            coroutine = this.GameController.SelectCardAndDoAction(
                new SelectCardDecision(this.GameController, this.HeroTurnTakerController, SelectionType.RemoveTokens, this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.IsInPlayAndHasGameText && c.FindTokenPool("FocusPool") != null && c.FindTokenPool("FocusPool").CurrentValue > 0)),
                (SelectCardDecision d) => this.GameController.RemoveTokensFromPool(d.SelectedCard.FindTokenPool("FocusPool"), 1, cardSource: this.GetCardSource()),
            false);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play a card.
            coroutine = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}