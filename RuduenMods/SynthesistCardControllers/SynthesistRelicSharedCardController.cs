using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public abstract class SynthesistRelicSharedCardController : CardController
    {
        public SynthesistRelicSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddIndividualTrigger();
            this.AddWhenDestroyedTrigger(OnDestroyResponse, new TriggerType[] { TriggerType.RemoveFromGame, TriggerType.ChangePostDestroyDestination });
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dc)
        {
            // Trigger should specifically only trigger on this particular card, and other destruction handling should otherwise be set. 
            if (dc.CardToDestroy.CanBeMovedOutOfGame)
            {
                this.AddInhibitorException((GameAction ga) => ga is MoveCardAction && (ga as MoveCardAction).Destination.IsOutOfGame);
                this.AddInhibitorException((GameAction ga) => ga is MessageAction && (ga as MessageAction).AssociatedCards.Contains(this.Card));
                this.AddInhibitorException((GameAction ga) => ga is TargetLeavesPlayAction);
                dc.SetPostDestroyDestination(dc.CardToDestroy.TurnTaker.OutOfGame, showMessage: true, cardSource: this.GetCardSource());
            }
            yield break;
        }
        protected abstract void AddIndividualTrigger();
    }
}