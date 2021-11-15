using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Soulbinder
{
    public class RitualOfKnowledgeCardController : SoulbinderSharedRitualCardController
    {
        public RitualOfKnowledgeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType[] RitualTriggerTypes { get { return new TriggerType[] { TriggerType.DealDamage }; } }

        protected override IEnumerator RitualCompleteResponse()
        {
            IEnumerator coroutine;
            coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 4, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}