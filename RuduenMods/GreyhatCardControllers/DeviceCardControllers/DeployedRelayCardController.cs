using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class DeployedRelayCardController : GreyhatSharedDeviceCardController
    {
        public DeployedRelayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayLinkFromTrashResponse, new TriggerType[] { TriggerType.PlayCard });
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator PlayLinkFromTrashResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Owner == this.TurnTaker && c.IsLink && c.Location == this.TurnTaker.Trash, true, false, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}