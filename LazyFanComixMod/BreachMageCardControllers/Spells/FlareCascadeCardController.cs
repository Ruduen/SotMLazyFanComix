using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.BreachMage
{
    // Manually tested!
    public class FlareCascadeCardController : BreachMageSharedSpellCardController
    {
        public FlareCascadeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ActivateCast()
        {
            IEnumerator coroutine;
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            // Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // You may destroy a charge
            coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController,
                new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("charge"), "charge", true, false, null, null, false),
                1, false, 0, null, storedResultsAction, null, false, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResultsAction.Count > 0 && storedResultsAction.FirstOrDefault().IsSuccessful)
            {
                // Activate this effect specifically.
                coroutine = this.GameController.ActivateAbility(this.GetActivatableAbilities("cast", this.HeroTurnTakerController).FirstOrDefault(), this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}