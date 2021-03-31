using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.BreachMage
{
    public abstract class BreachMageSharedSpellCardController : CardController
    {
        public BreachMageSharedSpellCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        // Shared NextTo logic: If possible, put it next to a breach! If not, the default handling of next to logic should take care of it.
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine = this.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("breach") && (c.FindTokenPool("FocusPool") == null || c.FindTokenPool("FocusPool").CurrentValue == 0), "breach"), storedResults, isPutIntoPlay, decisionSources);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator ActivateAbility(string abilityKey)
        {
            IEnumerator coroutine;
            if (abilityKey == "cast")
            {
                coroutine = this.ActivateCast();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected virtual IEnumerator ActivateCast()
        {
            IEnumerator coroutine = this.GameController.SendMessageAction("This spell isn't implemented properly yet. Blame Ruduen.", Priority.Medium, GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}