using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Soulbinder
{
    public class RitualOfSalvationCardController : SoulbinderSharedRitualCardController
    {
        public RitualOfSalvationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType[] RitualTriggerTypes { get { return new TriggerType[] { TriggerType.GainHP, TriggerType.DrawCard }; } }

        protected override IEnumerator RitualCompleteResponse()
        {
            IEnumerator coroutine;
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCards(new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, "active heroes"), 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}