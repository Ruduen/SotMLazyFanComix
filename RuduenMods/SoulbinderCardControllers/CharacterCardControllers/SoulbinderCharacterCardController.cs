using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public SoulbinderCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };

            // Deal Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal 1 projectile damage.

            // You may play a position. 
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardCriteria: new LinqCardCriteria((Card c) => c.IsPosition), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                // ADD APPROPRIATE TRIGGER for running start-of turn logic. 
                // ADD APPROPRIATE TRIGGER TO FLIP WHEN ALL CHARACTERS ARE INCAPACITATED. 
            }
            else
            {
                // ADD APPROPRIATE TRIGGER FOR FLIPPING BACK IF ONE OF THE CHARACTERS IS FLIPPED BACK.
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveAllTriggers();
            this.AddSideTriggers();
            yield break;
        }

        // TODO: Replace Incap with something more unique!
    }
}