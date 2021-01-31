using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public abstract class TrailblazerOnDestroyedPositionCardController : CardController
    {
        public TrailblazerOnDestroyedPositionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca)=>dca.CardToDestroy.Card.IsPosition && dca.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.ResponseAction), this.ResponseTriggerTypes(), TriggerTiming.After);
        }

        protected abstract IEnumerator ResponseAction(DestroyCardAction dca);
        protected abstract TriggerType[] ResponseTriggerTypes();

    }
}