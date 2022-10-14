using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class FlashStepBootsCardController : SharedEquipmentCardController
    {
        public FlashStepBootsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNum = this.GetPowerNumeral(0, 1);

            return this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, powerNum, false, 0, new LinqCardCriteria((Card c) => !this.IsEquipment(c)), cardSource: this.GetCardSource());
        }
    }
}