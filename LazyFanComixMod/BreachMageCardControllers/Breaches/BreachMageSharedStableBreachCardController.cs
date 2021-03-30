using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.BreachMage
{
    public abstract class BreachMageSharedStableBreachCardController : BreachMageSharedBreachController
    {
        public BreachMageSharedStableBreachCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseOpenPower()
        {
            // Play up to 3 spells.
            IEnumerator coroutine = this.SelectAndPlayCardsFromHand(this.DecisionMaker, 3, false, new int?(0), new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}