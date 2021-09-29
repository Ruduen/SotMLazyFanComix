using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Soulbinder
{
    public abstract class SoulbinderSharedSoulsplinterCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public SoulbinderSharedSoulsplinterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // You may play a sacrifice.
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("sacrifice"), "sacrifice"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals;
            switch (index)
            {
                case 1:
                    powerNumerals = new List<int>(){
                        this.GetPowerNumeral(0, 2),
                        this.GetPowerNumeral(1, 1)
                    };
                    IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
                    if (ritualTokenCards.Any())
                    {
                        // Remove 2 tokens from 1 ritual.
                        SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, powerNumerals[1], false, powerNumerals[1], true, cardSource: this.GetCardSource());
                        coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenResponse(scd, powerNumerals[0]), null, null, this.GetCardSource(), null, false, null);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else
                    {
                        coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    break;
                default:
                    coroutine = this.UseUniquePower();
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    break;
            }
        }
        private IEnumerator RemoveTokenResponse(SelectCardDecision scd, int number)
        {
            if (scd.SelectedCard != null && scd.SelectedCard.FindTokenPool("RitualTokenPool").CurrentValue > 0)
            {
                IEnumerator coroutine;

                coroutine = this.GameController.SendMessageAction("Removing " + number + " Ritual Token from " + scd.SelectedCard.AlternateTitleOrTitle + ".", Priority.Low, this.GetCardSource(), new Card[] { scd.SelectedCard }, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), number, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            yield break;
        }

        protected abstract IEnumerator UseUniquePower();
    }
}