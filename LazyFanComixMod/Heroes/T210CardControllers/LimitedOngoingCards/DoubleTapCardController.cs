using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.T210
{
    public class DoubleTapCardController : CardController
    {
        public DoubleTapCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == this.TurnTaker, Phase.UsePower, 1);
        }

        public override IEnumerator Play()
        {
            return this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1, null);
        }

        public override bool DoesHaveActivePlayMethod
        {
            get { return false; }
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            return this.GameController.ActiveTurnPhase.IsUsePower && this.GameController.ActiveTurnTaker == this.TurnTaker;
        }

    }
}