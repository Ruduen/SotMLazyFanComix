using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Bunker
{
    public class BunkerModeShiftCharacterCardController : PromoDefaultCharacterCardController
    {
        public BunkerModeShiftCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            if (this.TurnTaker.IsHero)
            {
                coroutine = this.SearchForCards(this.HeroTurnTakerController, true, true, 1, 1, new LinqCardCriteria((Card c) => c.IsMode, "mode", true, false, null, null, false), true, false, false);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.Card.Title + " has no deck or trash to search.", Priority.Medium, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Play a card. This is so Upgrade Mode results in a card play - the other modes will block it.
            coroutine = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}