using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public abstract class TrailblazerOnPlayPositionCardController : CardController
    {
        private const string _FirstPositionPlayedThisTurn = "FirstPositionPlayedThisTurn";
        public TrailblazerOnPlayPositionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa)=>!this.IsPropertyTrue(_FirstPositionPlayedThisTurn) && cepa.CardEnteringPlay.IsPosition && cepa.CardEnteringPlay.Owner == this.HeroTurnTaker, new Func<CardEntersPlayAction, IEnumerator>(this.Response), this.ResponseTriggerType(), TriggerTiming.After);
        }

        protected IEnumerator Response(CardEntersPlayAction cepa)
        {
            this.SetCardPropertyToTrueIfRealAction(_FirstPositionPlayedThisTurn);
            IEnumerator coroutine = ResponseAction(cepa);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected abstract IEnumerator ResponseAction(CardEntersPlayAction cepa);
        protected abstract TriggerType ResponseTriggerType();

    }
}