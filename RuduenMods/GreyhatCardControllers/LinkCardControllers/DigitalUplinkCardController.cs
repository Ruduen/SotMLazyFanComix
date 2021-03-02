using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

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
                // Damage.
                coroutine = this.GameController.SelectAndUsePower(this.GameController.FindCardController(nextTo).HeroTurnTakerControllerWithoutReplacements, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}