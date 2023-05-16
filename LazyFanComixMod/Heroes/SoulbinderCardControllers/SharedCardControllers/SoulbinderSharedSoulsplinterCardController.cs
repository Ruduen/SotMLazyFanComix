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
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("sacrifice"), "sacrifice"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<Card> target = new List<Card>();

            // TODO: Verify this works!
            List<int> powerNumerals = GetUniquePowerNumerals();

            // Do unique thing.
            coroutine = this.UseUniquePower(powerNumerals);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Third from last item is the self damage.
            coroutine = this.SelectYourTargetToDealDamage(target, powerNumerals[powerNumerals.Count - 3], DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (target.Count > 0)
            {
                // Deals themselves 2 damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, target.FirstOrDefault()), target.FirstOrDefault(), (Card c) => powerNumerals[powerNumerals.Count - 3], DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Last item is always number of cards, second to last is number of tokens.
            coroutine = this.RemoveTokenAction(powerNumerals[powerNumerals.Count - 1], powerNumerals[powerNumerals.Count - 2]);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RemoveTokenAction(int numRituals, int numTokens)
        {
            IEnumerator coroutine;
            // Do ritual based on power numerals
            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                // Remove X tokens from Y rituals.
                SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.HeroTurnTakerController, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, numRituals, false, numRituals, true, cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenResponse(scd, numTokens), null, null, this.GetCardSource(), null, false, null);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
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

        protected abstract IEnumerator UseUniquePower(List<int> powerNumerals);

        protected abstract List<int> GetUniquePowerNumerals();
    }
}