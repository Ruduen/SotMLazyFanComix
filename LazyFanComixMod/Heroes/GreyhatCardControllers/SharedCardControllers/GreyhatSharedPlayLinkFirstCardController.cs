using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

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

            // Play
            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, new LinqCardCriteria((Card c) => c.IsLink, "link"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = PostLinkPlay();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected abstract IEnumerator PostLinkPlay();
    }
}