using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class LandscapeAwarenessCardController : CardController
    {
        public LandscapeAwarenessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }
        public override bool DoesHaveActivePlayMethod
        {
            get { return false; }
        }

        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == this.TurnTaker, Phase.UsePower, 1);
        }
        public override IEnumerator Play()
        {
            return this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == this.TurnTaker, Phase.UsePower, 1);
        }
        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            return this.GameController.ActiveTurnPhase.IsUsePower && this.GameController.ActiveTurnTaker == this.TurnTaker;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 1);
            return this.GameController.DrawCards(this.DecisionMaker, powerNumeral, false, cardSource: this.GetCardSource());
        }

    }
}