using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop
{
    // Card controller for flippable cards with immediate effects on flip. Used because
    public class RuduenFlippableCardController : CardController
    {
        protected bool AddedFlipTriggers;
        protected List<ITrigger> FlipTriggers = new List<ITrigger>();
        protected List<ITrigger> SideTriggers = new List<ITrigger>();
        protected Dictionary<string, ITrigger> OneTimeTriggers = new Dictionary<string, ITrigger>();

        public RuduenFlippableCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override bool HasFlippedSide
        {
            get { return true; }
        }

        public override void AddTriggers()
        {
            this.AddSideTriggers();
        }

        public virtual void AddSideTriggers()
        {
        }

        public virtual void RemoveSideTriggers()
        {
            foreach (ITrigger trigger in this.SideTriggers)
            {
                this.RemoveTrigger(trigger, false);
            }
            this.SideTriggers.Clear();
        }

        protected void AddSideTrigger(ITrigger trigger)
        {
            this.SideTriggers.Add(trigger);
        }

        protected void AddSideTriggers(IEnumerable<ITrigger> triggers)
        {
            foreach (ITrigger trigger in triggers)
            {
                this.AddSideTrigger(trigger);
            }
        }

        protected void AddTriggerOnce(string identifier, ITrigger trigger)
        {
            if (!this.OneTimeTriggers.ContainsKey(identifier))
            {
                this.OneTimeTriggers.Add(identifier, base.AddTrigger(trigger));
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveSideTriggers();
            this.AddSideTriggers();
            yield break;
        }
    }
}