using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Thri
{
    public class ToolsOfTheTradeCardController : CardController
    {
        public ToolsOfTheTradeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Search Deck and play card, and shuffle.
            coroutine = this.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, this.TurnTaker.Deck, new LinqCardCriteria((Card c) => this.IsEquipment(c),"equipment"), new MoveCardDestination[] { new MoveCardDestination(this.TurnTaker.PlayArea) }, false, true, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Use a power.
            coroutine = this.GameController.SelectAndUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}