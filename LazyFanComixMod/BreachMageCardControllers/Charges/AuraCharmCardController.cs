using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.BreachMage
{
    // Manually tested!
    public class AuraCharmCardController : CardController
    {
        public AuraCharmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

            // Someone uses a power.
            coroutine = this.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, false, true, false, null, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.HeroTurnTaker), true, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}