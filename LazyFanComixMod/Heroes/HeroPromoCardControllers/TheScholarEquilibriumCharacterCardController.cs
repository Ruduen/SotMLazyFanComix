using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Linq;

namespace LazyFanComix.TheScholar
{
    public class TheScholarEquilibriumCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheScholarEquilibriumCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };
            IEnumerator coroutine;

            // Draw a card.
            coroutine = this.DrawCards(this.HeroTurnTakerController, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw an additional card for each elemental.
            coroutine = this.DrawCards(this.HeroTurnTakerController, this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsElemental).Count());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Trigger to increase damage dealt to self by 2 per elemental.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => ElementalDamage(numerals[1]));

            // Deal self 1 damage. Trigger will increase damage as necessary.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, numerals[0], DamageType.Psychic, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        private int ElementalDamage(int numeral)
        {
            return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsElemental).Count() * numeral;
        }
    }
}