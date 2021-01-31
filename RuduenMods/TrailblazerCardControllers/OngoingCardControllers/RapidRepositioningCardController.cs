using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class RapidRepositioningCardController : CardController
    {
        public RapidRepositioningCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay.IsEnvironment, this.ResponseAction, TriggerType.PlayCard, TriggerTiming.After);
        }

        protected IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> dcaResults = new List<DestroyCardAction>();

            // You may destroy a Position. If you do, play a card. 
            coroutine = this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsPosition), true, dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (dcaResults.Count > 0 && dcaResults.FirstOrDefault().WasCardDestroyed)
            {
                coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}