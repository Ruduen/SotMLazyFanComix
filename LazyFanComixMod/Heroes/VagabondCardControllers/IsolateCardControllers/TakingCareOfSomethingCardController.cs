using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

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
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsOngoing(c)), true, cardSource: this.GetCardSource()), TriggerType.DestroyCard);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), true, cardSource: this.GetCardSource()), TriggerType.DestroyCard);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 7), true, cardSource: this.GetCardSource()), TriggerType.DestroyCard);
        }
    }
}