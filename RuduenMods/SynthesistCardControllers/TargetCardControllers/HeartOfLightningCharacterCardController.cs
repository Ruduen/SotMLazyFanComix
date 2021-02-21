using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public class HeartOfLightningCharacterCardController : SynthesistSharedMultiCharacterCardController
    {
        public HeartOfLightningCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override void AddIndividualTrigger()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsSameCard(this.Card), 1);
        }
    }
}