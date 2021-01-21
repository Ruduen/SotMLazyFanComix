using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Cassie
{
    public class WaterlogCardController : CassieRiverSharedCardController
    {
        public WaterlogCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Select and Destroy card.
            IEnumerator coroutine = this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment", true, false, null, null, false), false, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}