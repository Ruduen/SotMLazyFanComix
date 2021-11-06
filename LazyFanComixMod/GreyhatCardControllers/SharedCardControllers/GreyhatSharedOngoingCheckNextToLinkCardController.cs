using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public abstract class GreyhatSharedOngoingCheckNextToLinkCardController : GreyhatSharedCheckNextToLinkCardController
    {

        public GreyhatSharedOngoingCheckNextToLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Play
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => c.IsLink, "link"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}