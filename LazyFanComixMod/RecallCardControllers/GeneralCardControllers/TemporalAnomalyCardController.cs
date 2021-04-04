using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class TemporalAnomalyCardController : CardController
    {
        public TemporalAnomalyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.SearchForCards(this.DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("loop") || c.DoKeywordsContain("paradox"), "loop or paradox"), false, true, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}