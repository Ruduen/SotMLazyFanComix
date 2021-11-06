using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AbsoluteZero
{
    public class AbsoluteZeroOverloadCharacterCardController : PromoDefaultCharacterCardController
    {
        public AbsoluteZeroOverloadCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>(){
                this.GetPowerNumeral(0, 3), // Amount of damage.
                this.GetPowerNumeral(1, 4) // Cards Discarded.
            };
            IEnumerator coroutine;

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.Card), this.Card, powerNumerals[0], DamageType.Fire, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.TurnTaker.IsHero)
            {
                coroutine = this.GameController.DiscardTopCards(this.HeroTurnTakerController, this.TurnTaker.Deck, powerNumerals[1], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.Card.AlternateTitleOrTitle + " has no deck to discard from.", Priority.Low, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndPlayCard(this.HeroTurnTakerController, (Card c) => c.IsModule && c.Location == this.TurnTaker.Trash, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DrawAndDestroyCoroutine(int numeral)
        {
            IEnumerator coroutine;
            coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, numeral, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, cardCriteria: new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && this.IsEquipment(c), "equipment"), false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


    }
}