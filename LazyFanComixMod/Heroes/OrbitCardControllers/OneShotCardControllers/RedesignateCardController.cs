using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class RedesignateCardController : CardController
    {
        public RedesignateCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<MoveCardAction> mcaResults = new List<MoveCardAction>();

            coroutine = this.GameController.SelectAndReturnCards(this.DecisionMaker, null, new LinqCardCriteria((Card c) => c.IsCover, "cover"), true, false, false, 0, mcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (mcaResults?.Any((MoveCardAction mca) => mca.WasCardMoved) == true)
            {
                coroutine = this.GameController.DrawCards(this.DecisionMaker, mcaResults.Count((MoveCardAction mca) => mca.WasCardMoved), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 2, false, 0, new LinqCardCriteria((Card c) => c.IsCover, "cover"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}