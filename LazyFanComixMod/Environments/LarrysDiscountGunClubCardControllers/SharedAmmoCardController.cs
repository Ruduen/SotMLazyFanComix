using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public abstract class SharedAmmoCardController : AmmoCardController
    {
        public SharedAmmoCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public abstract override IEnumerator BeforeDamageResponse();

        public override IEnumerator RunIfUnableToEnterPlay()
        {
            IEnumerator coroutine;

            if (!this.TurnTaker.Deck.HasCards)
            {
                coroutine = this.GameController.SendMessageAction("There are no Gun cards with room for " + this.Card.Title + ", but the Environment Deck is empty, so no card will be played.", Priority.High, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no Gun cards with room for " + this.Card.Title + ". Playing the top card of the environment deck...", Priority.Medium, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, false, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}