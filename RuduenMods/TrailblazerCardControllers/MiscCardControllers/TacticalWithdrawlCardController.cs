using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class TacticalWithdrawlCardController : CardController
    {
        public TacticalWithdrawlCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            coroutine = this.GameController.SelectCardAndDoAction(
                new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.UsePower, this.GameController.FindCardsWhere((Card c) => c.IsPosition && c.IsInPlay && c.HasPowers && c.Owner == this.HeroTurnTaker), cardSource: this.GetCardSource()),
                (SelectCardDecision scd) => this.UseSelectedCardPowerTwice(scd)
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator UseSelectedCardPowerTwice(SelectCardDecision scd)
        {
            IEnumerator coroutine;

            // If there is a position with a power... 
            if (scd.SelectedCard != null)
            {
                // Select the card and use the power twice. Again, this should be fine without more complex checks due to restrictions. 

                coroutine = this.GameController.UsePower(scd.SelectedCard, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.UsePower(scd.SelectedCard, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DestroyCard(this.DecisionMaker, scd.SelectedCard, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}