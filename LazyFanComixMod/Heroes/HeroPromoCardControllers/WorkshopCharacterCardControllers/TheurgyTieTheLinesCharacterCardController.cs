using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Theurgy
{
    public class TheurgyTieTheLinesCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheurgyTieTheLinesCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.HeroTurnTaker.Hand, new LinqCardCriteria((Card c) => this.GameController.DoesCardContainKeyword(c, "charm")));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, powerNums[0], false, powerNums[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => this.GameController.DoesCardContainKeyword(c, "charm")), 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}