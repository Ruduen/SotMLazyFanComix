using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class TakingCareOfSomethingCardController : SharedIsolateCardController
    {
        public TakingCareOfSomethingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment || (c.IsTarget && c.HitPoints <= 5)), true, cardSource: this.GetCardSource()), TriggerType.DestroyCard);
        }
    }
}