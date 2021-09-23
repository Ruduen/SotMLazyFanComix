using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class DigitalUplinkCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c != this.CharacterCard, "hero character"); } }

        public DigitalUplinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            Card nextTo = this.GetCardThisCardIsNextTo(true);
            IEnumerator coroutine;
            if (nextTo != null)
            {
                // Discard.
                List<DiscardCardAction> dcaResults = new List<DiscardCardAction>();
                HeroTurnTakerController httc = this.GameController.FindCardController(nextTo).HeroTurnTakerControllerWithoutReplacements;

                // httc is optional due to Celestial Tribunal, so only proceed if it was found. 
                if (httc != null)
                {
                    coroutine = this.GameController.SelectAndDiscardCards(httc, 1, false, 0, dcaResults, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    if (dcaResults.Count > 0 && dcaResults.FirstOrDefault().WasCardDiscarded)
                    {
                        // Power.
                        coroutine = this.GameController.SelectAndUsePower(httc, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }
                else
                {
                    coroutine = this.GameController.SendMessageAction(nextTo.Title + " does not have a hand, and therefore cannot discard any cards.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}