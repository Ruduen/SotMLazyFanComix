using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class BreakTheKeystoneCardController : SharedSoloCardController
    {
        public BreakTheKeystoneCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing), 2, false, 0, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.GameController.SelectAndUsePower(this.DecisionMaker, true, null, 2, cardSource: this.GetCardSource());
        }

        protected override string IfNotSoloMessage()
        {
            return "no powers can be used";
        }
    }
}