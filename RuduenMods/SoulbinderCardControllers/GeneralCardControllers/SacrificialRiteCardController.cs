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
            List<Card> actedRituals = new List<Card>();

            // You may play a ritual or soulsplinter.
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("ritual") || c.DoKeywordsContain("soulsplinter"), "ritual or soulsplinter"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, 2, DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals another 2 damage.
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, targetList.FirstOrDefault()), 2, DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // That target deals itself 2 Infernal damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), 2, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Remove a token from a ritual.
            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                // Each valid ritual loses a token. This needs to be done to handle scenarios where things are destroyed or played mid-way!
                SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, null, false, null, true, true, false, () => NumRitualsToRemoveToken(actedRituals), null, null, null, this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenEachResponse(scd, actedRituals, 1), null, null, this.GetCardSource(), null, false, null);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator RemoveTokenEachResponse(SelectCardDecision scd, List<Card> actedTargets, int numeral)
        {
            if (scd.SelectedCard != null && scd.SelectedCard.FindTokenPool("RitualTokenPool").CurrentValue > 0)
            {
                actedTargets.Add(scd.SelectedCard);
                IEnumerator coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), numeral, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private int NumRitualsToRemoveToken(List<Card> actedTargets)
        {
            if (!this.TurnTaker.IsIncapacitatedOrOutOfGame)
            {
                int num = this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0).Except(actedTargets).Count();
                return actedTargets.Count() + num;
            }
            return 0;
        }
    }
}