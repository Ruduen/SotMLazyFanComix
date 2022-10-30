using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ImbuedDetonationCardController : SharedSoloCardController
    {
        public ImbuedDetonationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.DealDamage(this.CharacterCard, (Card c) => true, 3, DamageType.Fire);
        }

        protected override IEnumerator OnPlayIfNotSolo()
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Fire, 1, false, 1, cardSource: this.GetCardSource());
        }
    }
}