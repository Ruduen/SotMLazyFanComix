using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class MantleOfReposeCardController : SharedEquipmentCardController
    {
        public MantleOfReposeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNum = this.GetPowerNumeral(0, 3);

            return this.GameController.GainHP(this.CharacterCard, powerNum, cardSource: this.GetCardSource());
        }
    }
}