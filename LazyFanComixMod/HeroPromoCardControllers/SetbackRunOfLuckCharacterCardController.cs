using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Setback
{
    public class SetbackRunOfLuckCharacterCardController : PromoDefaultCharacterCardController
    {
        public SetbackRunOfLuckCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]{
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 2)
            };
            IEnumerator coroutine;

            coroutine = this.GameController.AddTokensToPool(this.Card.FindTokenPool(TokenPool.UnluckyPoolIdentifier), powerNumerals[0], cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[1], cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.Card.FindTokenPool(TokenPool.UnluckyPoolIdentifier).CurrentValue > this.HeroTurnTaker.NumberOfCardsInHand)
            {
                coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, powerNumerals[2], false, cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}