using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public abstract class SharedEquipmentCardController : CardController
    {
        public SharedEquipmentCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            return this.GameController.UsePower(this.Card, 0, cardSource: this.GetCardSource());
        }

    }
}