using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class RitualOfCausalityCardController : SoulbinderSharedRitualCardController
    {
        public RitualOfCausalityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType[] RitualTriggerTypes { get { return new TriggerType[] { TriggerType.DealDamage }; } }

        protected override IEnumerator RitualCompleteResponse()
        {
            IEnumerator coroutine;
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}