using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class SacrificialRiteCardController : SoulbinderSharedYourTargetDamageCardController
    {

        public SacrificialRiteCardController(Card card, TurnTakerController turnTakerController)
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

            if (targetList.Count > 0 )
            {
                // That target deals another 3 damage.
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, targetList.FirstOrDefault()), 2, DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // That target deals itself 3 Infernal damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), 2, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Remove a token from a ritual.
            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                coroutine = this.GameController.SelectCardAndDoAction(
                    new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.RemoveTokens, ritualTokenCards, cardSource: this.GetCardSource()), (SelectCardDecision scd) => RemoveTokenFromRitual(scd, 1));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator RemoveTokenFromRitual(SelectCardDecision scd, int number)
        {
            if (scd.SelectedCard != null)
            {
                IEnumerator coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), number, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            yield break;
        }

    }
}