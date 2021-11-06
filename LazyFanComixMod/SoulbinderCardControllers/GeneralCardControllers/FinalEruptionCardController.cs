using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public class FinalEruptionCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public FinalEruptionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> targetList = new List<Card>();

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, null, DamageType.Infernal, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                if (targetList.FirstOrDefault().HitPoints != null)
                {
                    coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, targetList.FirstOrDefault()), (int)targetList.FirstOrDefault().HitPoints + 2, DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }

                // That target is destroyed.
                coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, targetList.FirstOrDefault(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}