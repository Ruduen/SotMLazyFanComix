using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ClandestineExecutionerCardController : SharedIsolateCardController
    {
        public ClandestineExecutionerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsSameCard(this.CharacterCard), 1);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndUsePower(this.DecisionMaker, true, cardSource: this.GetCardSource()), TriggerType.MoveCard);
        }
    }
}