using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Cassie
{
    public class MeetingTheOceanCardController : CassieRiverSharedCardController
    {
        public MeetingTheOceanCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>() {
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 3),
            };
            int totalDamage = 0;

            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine;
            coroutine = this.SelectAndDiscardCards(this.DecisionMaker, powerNumerals[0], false, 0, storedResults, false, null, null, null, SelectionType.DiscardCard, this.TurnTaker);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (DiscardCardAction discardCardAction in storedResults)
            {
                if (discardCardAction.IsSuccessful && discardCardAction.CardToDiscard.MagicNumber != null)
                {
                    totalDamage += (int)discardCardAction.CardToDiscard.MagicNumber;
                }
            }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), totalDamage, DamageType.Cold, powerNumerals[1], false, powerNumerals[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw until you have 3 cards.
            coroutine = this.DrawCardsUntilHandSizeReached(this.DecisionMaker, powerNumerals[2]);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}