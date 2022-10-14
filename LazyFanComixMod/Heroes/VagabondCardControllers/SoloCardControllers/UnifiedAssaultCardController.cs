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
            return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.UsePower, (TurnTaker tt) => this.GameController.SelectAndUsePower(this.GameController.FindHeroTurnTakerController(tt.ToHero()), true, cardSource: this.GetCardSource()), 2, false, 0, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {

            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 4, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
        }

        protected override string IfNotSoloMessage()
        {
            return "no damage will be dealt";
        }
    }
}