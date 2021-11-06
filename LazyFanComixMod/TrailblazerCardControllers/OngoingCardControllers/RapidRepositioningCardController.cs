using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Trailblazer
{
    public class RapidRepositioningCardController : CardController
    {
        private const string _FirstEnvironmentPlayedThisTurn = "_FirstEnvironmentPlayedThisTurn";

        public RapidRepositioningCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstEnvironmentPlayedThisTurn, "An Environment card has already been played this turn.", "An Environment card has not yet been played this turn.");
        }

        public override void AddTriggers()
        {
            this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => !this.IsPropertyTrue(_FirstEnvironmentPlayedThisTurn) && cepa.CardEnteringPlay.IsEnvironment, this.ResponseAction, TriggerType.PlayCard, TriggerTiming.After);
        }

        protected IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            this.SetCardPropertyToTrueIfRealAction(_FirstEnvironmentPlayedThisTurn);
            IEnumerator coroutine;
            List<DestroyCardAction> dcaResults = new List<DestroyCardAction>();

            // You may destroy a Position. If you do, play a card.
            coroutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsPosition), true, dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dcaResults.Count > 0 && dcaResults.FirstOrDefault().WasCardDestroyed)
            {
                coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}