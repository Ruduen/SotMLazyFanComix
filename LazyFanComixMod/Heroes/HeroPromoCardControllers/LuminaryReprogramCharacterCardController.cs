using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Luminary
{
    public class LuminaryReprogramCharacterCardController : PromoDefaultCharacterCardController
    {
        public LuminaryReprogramCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 4),
                this.GetPowerNumeral(3, 2)
            };
            List<DealDamageAction> ddaResults = new List<DealDamageAction>();

            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Lightning, powerNumerals[0], false, powerNumerals[0], storedResultsDamage: ddaResults, cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Get all damaged cards that are devices.
            IEnumerable<Card> devices = ddaResults.Where((DealDamageAction dda) => dda.DidDealDamage && dda.Target.IsDevice).Select((DealDamageAction dda) => dda.Target);

            if (devices.Count() > 0)
            {
                // Targets in the Device list deal damage to themselves.
                coroutine = this.GameController.SelectTargetsToDealDamageToSelf(this.HeroTurnTakerController, powerNumerals[2], DamageType.Lightning, null, false, null, additionalCriteria: (Card c) => devices.Contains(c), cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Draw 2 cards.
                coroutine = this.DrawCards(this.HeroTurnTakerController, powerNumerals[3]);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // You may play a device.
                coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true,
                    cardCriteria: new LinqCardCriteria((Card c) => c.IsDevice, "device"),
                    cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}