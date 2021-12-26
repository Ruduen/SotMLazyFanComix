using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.BreachMage
{
    // Manually tested!
    public class BreachMageRenegadeCharacterCardController : BreachMageSharedCharacterCardController
    {
        protected override int[] BreachInitialFocus { get { return new int[] { -1, 4, 3, 1 }; } }
        public BreachMageRenegadeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]{
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };

            IEnumerator coroutine;
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            // Charge ability attempt.
            // Destroy one of your charges.
            coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController,
                new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.TurnTaker && c.DoKeywordsContain("charge"), "charge", true, false, null, null, false),
                powerNumerals[0], false, null, null, storedResultsAction, null, false, null, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.GetNumberOfCardsDestroyed(storedResultsAction) == powerNumerals[0])
            {
                // If one were destroyed, all hero targets regain 3.
                coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero, 3, cardSource: this.GetCardSource());
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
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}