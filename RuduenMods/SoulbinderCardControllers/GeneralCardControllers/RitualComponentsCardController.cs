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
                this.GetPowerNumeral(0, 1), // 1 Token from a Ritual
                this.GetPowerNumeral(1, 3), // At least 3 targets
                this.GetPowerNumeral(2, 1)  // 1 Token from each ritual.
            };

            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                // Remove 1 token from 1 Ritual.
                coroutine = this.GameController.SelectCardAndDoAction(
                    new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.RemoveTokens, ritualTokenCards, cardSource: this.GetCardSource()), (SelectCardDecision scd) => RemoveTokenResponse(scd, powerNumerals[0]));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // If there are at least 3 targets...
                if (FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.Owner == this.TurnTaker && c.IsTarget).Count() >= powerNumerals[1])
                {
                    // Each valid ritual loses a token. This needs to be done to handle scenarios where things are destroyed or played mid-way!
                    SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, null, false, null, true, true, false, () => NumRitualsToRemoveToken(actedTargets), null, null, null, this.GetCardSource());
                    coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenEachResponse(scd, actedTargets, powerNumerals[2]), null, null, this.GetCardSource(), null, false, null);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }


        private IEnumerator RemoveTokenEachResponse(SelectCardDecision scd, List<Card> actedTargets, int numeral)
        {
            if (scd.SelectedCard != null)
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