using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public abstract class SharedOrbitalCardController : CardController
    {
        public SharedOrbitalCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            return this.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget), storedResults, isPutIntoPlay, decisionSources);
        }

        public override IEnumerator Play()
        {
            Card nextTo = this.GetCardThisCardIsNextTo();
            if (nextTo == null) { return null; }

            return this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => c == nextTo, 2, DamageType.Melee, cardSource: this.GetCardSource());
        }

        public override void AddTriggers()
        {
            this.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(true, true);
            this.AddUniqueTriggers();
        }

        public abstract void AddUniqueTriggers();


    }
}