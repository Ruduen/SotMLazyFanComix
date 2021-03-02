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

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, 2, DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals itself 2 Infernal damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), 2, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            IEnumerable<string> ritualsInPlay = this.GameController.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual")).Select((Card c) => c.Identifier);
            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ritual") && !ritualsInPlay.Contains(c.Identifier)), 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}