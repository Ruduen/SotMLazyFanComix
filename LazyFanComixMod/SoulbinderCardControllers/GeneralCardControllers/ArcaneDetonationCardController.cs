using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public class ArcaneDetonationCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public ArcaneDetonationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> targetList = new List<Card>();

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, 2, DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals all others 2 Infernal damage.
                coroutine = this.GameController.DealDamage(this.DecisionMaker, targetList.FirstOrDefault(), (Card c) => !c.IsHero, 2, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // That target deals itself 2 Infernal damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), 2, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}