using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Linq;

namespace RuduenWorkshop.Unity
{
    public class UnitySparePartsCharacterCardController : PromoDefaultCharacterCardController
    {
        public UnitySparePartsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numeral = this.GetPowerNumeral(0, 2);

            IEnumerator coroutine;

            // Draw a card.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If this hero has at least 2 equipments in play, play a card.
            if (this.GameController.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && IsEquipment(c) && c.IsInPlayAndNotUnderCard).Count() >= numeral)
            {
                coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}