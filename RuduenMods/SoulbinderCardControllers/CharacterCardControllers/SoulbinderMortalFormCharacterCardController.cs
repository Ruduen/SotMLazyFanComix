using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderMortalFormCharacterCardController : SoulbinderSharedMultiCharacterCardController
    {

        protected string[] ShardIdentifiers { get { return new string[] { "SoulshardOfLightningCharacter", "SoulshardOfMercuryCharacter", "SoulshardOfIronCharacter" }; } }

        public SoulbinderMortalFormCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                // General logic for flipping when incapping this.
                this.AddSideTrigger(
                    this.AddTrigger<FlipCardAction>(
                        (FlipCardAction fca) => fca.CardToFlip.Card.Identifier == this.Card.Identifier && !fca.CardToFlip.Card.IsFlipped,
                        (FlipCardAction fca) => RemoveOtherRelicsResponse(),
                        new TriggerType[] { TriggerType.FlipCard, TriggerType.GainHP }, TriggerTiming.Before
                    )
                );
            }
        }

        private IEnumerator RemoveOtherRelicsResponse()
        {
            IEnumerator coroutine = this.GameController.MoveCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && c.IsInPlay && ShardIdentifiers.Contains(c.Identifier)), (Card c) => this.TurnTaker.OutOfGame, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };

            coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                coroutine = this.GameController.SelectCardAndDoAction(
                    new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.RemoveTokens, ritualTokenCards, cardSource: this.GetCardSource()), (SelectCardDecision scd) => RemoveTokenFromRitual(scd, powerNumerals[1]));
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