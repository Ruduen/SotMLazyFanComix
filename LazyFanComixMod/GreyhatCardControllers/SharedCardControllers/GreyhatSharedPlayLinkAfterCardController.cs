using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public abstract class GreyhatSharedPlayLinkFirstCardController : GreyhatSharedCheckNextToLinkCardController
    {
        public GreyhatSharedPlayLinkFirstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            // Play
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => c.IsLink, "link"), storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = PostLinkPlay();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }

        protected abstract IEnumerator PostLinkPlay();
    }
}