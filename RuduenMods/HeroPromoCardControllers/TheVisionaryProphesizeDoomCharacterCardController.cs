using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.TheVisionary
{
    public class TheVisionaryProphesizeDoomCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheVisionaryProphesizeDoomCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[] {
                GetPowerNumeral(0, 2), // 2 Cards Beneath
                GetPowerNumeral(1, 1), // 1 Target
                GetPowerNumeral(2, 4), // 4 Damage
                GetPowerNumeral(3, 2)  // Top 2 cards
            };
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine;

            if (this.Card.UnderLocation.NumberOfCards >= powerNumerals[0])
            {
                // Destroy all cards under this.
                // TODO: Can AutoDecide do this?
                coroutine = this.GameController.DestroyCards(this.DecisionMaker,
                    new LinqCardCriteria(
                        (Card c) => c.Location == this.Card.UnderLocation || c.Location == this.Card.BelowLocation, "cards below " + this.Card.Title),
                    cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Select a target for the environment to deal damage.
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.FindEnvironment().TurnTaker), powerNumerals[2], DamageType.Projectile, powerNumerals[1], false, powerNumerals[1], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Move the top 2 cards under this. Normal empty deck logic should work if it doesn't.
                coroutine = this.GameController.MoveCards(this.DecisionMaker, this.HeroTurnTaker.Deck, this.Card.UnderLocation, powerNumerals[3]);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Play one of the cards under/below this. Yes, both must be checked based on Omnicannon.
                coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.Card.UnderLocation || c.Location == this.Card.BelowLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}