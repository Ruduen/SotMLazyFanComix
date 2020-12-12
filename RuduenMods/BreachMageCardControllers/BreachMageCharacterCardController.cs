using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    // Manually tested!
    public class BreachMageCharacterCardController : BreachMageSharedCharacterCardController
    {

        public BreachMageCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            BreachInitialFocus = new int[] { 0, 0, 3, 4 };

            this.SpecialStringMaker.ShowSpecialString(() => string.Format("The breaches have Focus Pools of: {0}",
                new object[] { GetBreachFocusPoolString() }),
                null, null);
        }

        public string GetBreachFocusPoolString()
        {
            string poolString = "";
            IEnumerable<TokenPool> focusPools = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("open breach") || c.DoKeywordsContain("closed breach", false, true)).Select((Card c) => c.FindTokenPool("FocusPool"));

            foreach (TokenPool focusPool in focusPools)
            {
                if (poolString != "") { poolString += ", "; }
                poolString += focusPool.CurrentValue.ToString();
            }
            if (poolString == "")
            {
                poolString = "N/A";
            }
            return poolString;
        }

        public override IEnumerator UsePower(int index = 0)
        {

            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 4)
            };

            IEnumerator coroutine;
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            // Charge ability attempt.
            // Destroy two of your charges.
            coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker,
                new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("charge"), "charge", true, false, null, null, false),
                powerNumerals[0], false, null, null, storedResultsAction, null, false, null, null, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.GetNumberOfCardsDestroyed(storedResultsAction) == powerNumerals[0])
            {
                // If two were destroyed, someone draws 5.
                coroutine = this.GameController.SelectHeroToDrawCards(this.DecisionMaker, powerNumerals[1], false, false, null, false, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes"), null, null, this.GetCardSource(null));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
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
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }

        public IEnumerator CastAndDestroySpell(HeroTurnTakerController heroTurnTakerController)
        {
            IEnumerator coroutine;
            // Stanard power.
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

            // Use a Cast.
            //coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", null, storedResults);
            coroutine = this.GameController.SelectAndActivateAbility(heroTurnTakerController, "cast", null, storedResults, false, this.GetCardSource(null));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0)
            {
                // Destroy the cast card.
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, storedResults.FirstOrDefault().SelectedCard);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}