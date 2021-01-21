using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachMageTwincasterCharacterCardController : BreachMageSharedCharacterCardController
    {
        public string str;

        public BreachMageTwincasterCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            BreachInitialFocus = new int[] { 0, 0, 3, 4 };
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Break down into two powers.
            IEnumerator coroutine;
            int powerNumeral = this.GetPowerNumeral(0, 1); // Number of charges.

            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();

            // Destroy 1 of your charges.
            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker,
                new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("charge"), "charge", true, false, null, null, false),
                powerNumeral, false, null, null, storedResultsAction, null, false, null, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.GetNumberOfCardsDestroyed(storedResultsAction) == powerNumeral)
            {
                // If two were destroyed, select use a spell twice.
                List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

                // Use a Cast.
                //coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", null, storedResults);
                coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", null, storedResults, false, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0)
                {
                    // Select an ability on that card. Just in case a card has multiple Cast effects, or is no longer valid due to being destroyed.
                    coroutine = this.ActivateCast(storedResults.FirstOrDefault().SelectedCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    // Destroy the cast card.
                    coroutine = this.GameController.DestroyCard(this.DecisionMaker, storedResults.FirstOrDefault().SelectedCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }

        // TODO: Replace with something more unique!
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.DecisionMaker);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}