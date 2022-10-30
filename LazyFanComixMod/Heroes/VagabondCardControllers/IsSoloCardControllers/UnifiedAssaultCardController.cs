using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class UnifiedAssaultCardController : SharedSoloCardController
    {
        public UnifiedAssaultCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectAndUsePower(this.DecisionMaker, true, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {

            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfNotSolo()
        {
            return this.GameController.SelectHeroToUsePower(this.DecisionMaker, true, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker), cardSource: this.GetCardSource());
        }
    }
}