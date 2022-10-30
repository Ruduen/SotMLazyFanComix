using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class AlchemicCollectorCardController : SharedSoloCardController
    {
        public AlchemicCollectorCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.GainHP(this.DecisionMaker, (Card c) => c.Owner == this.TurnTaker, 2, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Fire, 2, false, 0, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfNotSolo()
        {
            return this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero && c.Owner != this.TurnTaker, 2, cardSource: this.GetCardSource());
        }
    }
}