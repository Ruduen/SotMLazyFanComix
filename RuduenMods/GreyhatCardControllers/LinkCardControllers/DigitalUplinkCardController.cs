using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
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
                coroutine = this.GameController.SelectAndDiscardCards(httc, 1, false, 0, dcaResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (dcaResults.Count > 0 && dcaResults.FirstOrDefault().WasCardDiscarded)
                {
                    // Power.
                    coroutine = this.GameController.SelectAndUsePower(httc, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}