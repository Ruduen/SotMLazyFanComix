using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class GreyhatCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public GreyhatCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[]{
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 1),
            };
            IEnumerator coroutine;

            if (this.TurnTaker.IsHero)
            {
                coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, numerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, numerals[1], false, numerals[1], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.Card.Title + " has no cards to draw or discard.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        // TODO: Replace Incap with something more unique!
    }
}