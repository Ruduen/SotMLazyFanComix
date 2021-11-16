using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class TheRealmsChoiceCardController : CardController
    {
        public TheRealmsChoiceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger(
                (TurnTaker tt) => tt == this.TurnTaker,
                (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c)=>c.IsEnvironment),1,false,1,cardSource: this.GetCardSource()),
                TriggerType.DestroyCard
            );
            this.AddEndOfTurnTrigger(
                (TurnTaker tt) => tt == this.TurnTaker,
                (PhaseChangeAction pca) => this.GameController.PlayTopCard(this.DecisionMaker, this.FindEnvironment(), false, 2, showMessage: true, cardSource: this.GetCardSource()),
                TriggerType.PlayCard
            );
        }
    }
}
