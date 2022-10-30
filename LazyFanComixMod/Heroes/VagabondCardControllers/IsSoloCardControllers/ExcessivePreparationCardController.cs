using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ExcessivePreparationCardController : SharedSoloCardController
    {
        public ExcessivePreparationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), 1, false, 0, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.GameController.SelectAndUsePower(this.DecisionMaker, true, null, 2, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfNotSolo()
        {
            return this.SelectHeroToPlayCard(this.DecisionMaker, true, true, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker));
        }
    }
}