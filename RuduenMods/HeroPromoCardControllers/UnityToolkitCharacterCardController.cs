using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;

namespace RuduenWorkshop.Unity
{
    public class UnityToolkitCharacterCardController : PromoDefaultCharacterCardController
    {
        public UnityToolkitCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.HeroTurnTakerController, this.HeroTurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.HasPowers, "power-containing"), 1, showMessage: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}