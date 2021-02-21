using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public class FrameOfIronCharacterCardController : SynthesistSharedMultiCharacterCardController
    {
        public FrameOfIronCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        protected override void AddIndividualTrigger()
        {
            this.AddReduceDamageTrigger((Card c) => c == this.Card, 1);
        }

    }
}