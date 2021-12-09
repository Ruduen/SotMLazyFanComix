using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
				coroutine = this.GameController.SendMessageAction("There are no Gun cards in play to put " + this.Card.Title + " next to, but the Environment Deck is empty, so no card will be played.", Priority.High, this.GetCardSource(), null, true);
				if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
			}
            else
			{
				coroutine = this.GameController.SendMessageAction("There are no Gun cards in play to put " + this.Card.Title + " next to. Playing the top card of the environment deck...", Priority.Medium, this.GetCardSource(), null, true);
				if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

				coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, false, 1, cardSource: this.GetCardSource());
				if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
			}
		}
    }
}