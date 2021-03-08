using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public abstract class GreyhatSharedOngoingCheckNextToLinkCardController : GreyhatSharedCheckNextToLinkCardController
    {

        public GreyhatSharedOngoingCheckNextToLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public abstract void AddUniqueTriggers();
        public override void AddTriggers()
        {
            this.AddUniqueTriggers();
        }

        public override IEnumerator Play()
        {
            // You may play a link.
            IEnumerator coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 1, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.IsLink, "link"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}