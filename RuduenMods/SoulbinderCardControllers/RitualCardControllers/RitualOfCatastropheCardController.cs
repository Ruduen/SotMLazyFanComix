using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Soulbinder
{
    public class RitualOfCatastropheCardController : SoulbinderSharedRitualCardController
    {
        public RitualOfCatastropheCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType[] RitualTriggerTypes { get { return new TriggerType[] { TriggerType.DealDamage }; } }

        protected override IEnumerator RitualCompleteResponse()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c != this.Card && (c.IsOngoing || c.IsEnvironment), "ongoing or environment"), 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => !c.IsHero, 3, DamageType.Infernal, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}