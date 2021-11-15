using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public class RitualOfTransferrenceCardController : SoulbinderSharedRitualCardController
    {
        public RitualOfTransferrenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType[] RitualTriggerTypes { get { return new TriggerType[] { TriggerType.DealDamage }; } }

        protected override IEnumerator RitualCompleteResponse()
        {
            List<GainHPAction> ghpaResults = new List<GainHPAction>();

            IEnumerator coroutine;
            coroutine = this.GameController.SelectAndGainHP(this.HeroTurnTakerController, 2, false, null, 3, 0, false, ghpaResults, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            int amountHealed = ghpaResults.Sum((GainHPAction ghpa) => ghpa.AmountActuallyGained);

            coroutine = this.GameController.SelectTargetsToDealDamageToSelf(this.HeroTurnTakerController, amountHealed, DamageType.Toxic, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}