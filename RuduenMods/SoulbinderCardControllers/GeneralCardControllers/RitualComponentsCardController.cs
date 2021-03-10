using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class RitualComponentsCardController : CardController
    {
        public RitualComponentsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<Card> actedTargets = new List<Card>();
            // Select target to deal damage to.

            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1), // 1 Target
                this.GetPowerNumeral(1, 2), // 2 Damage
                this.GetPowerNumeral(2, 1), // 1 Token from
                this.GetPowerNumeral(3, 1)  // 1 Ritual
            };

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());

            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                // Each valid ritual loses a token. This needs to be done to handle scenarios where things are destroyed or played mid-way!
                SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, powerNumerals[3], false, powerNumerals[3], true, cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenResponse(scd, powerNumerals[2]), null, null, this.GetCardSource(), null, false, null);
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
            if (scd.SelectedCard != null)
            {
                IEnumerator coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), number, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            yield break;
        }
    }
}