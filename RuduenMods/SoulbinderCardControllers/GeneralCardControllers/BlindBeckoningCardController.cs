using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class BlindBeckoningCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public BlindBeckoningCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> targetList = new List<Card>();

            IEnumerable<string> ritualsInPlay = this.GameController.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual")).Select((Card c) => c.Identifier);
            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ritual") && !ritualsInPlay.Contains(c.Identifier)), 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, this.GameController.FindCardsWhere((Card c)=>c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual")).Count(), DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals one enemy (rituals) damage. 
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, targetList.FirstOrDefault()), this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual")).Count(), DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // That target deals itself (rituals) damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual")).Count(), DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}