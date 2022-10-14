using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ImproviseCardController : SharedSoloCardController
    {
        public ImproviseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero), SelectionType.DrawCard, (TurnTaker tt) => this.GameController.DrawCard(tt.ToHero(), true, cardSource: this.GetCardSource()), 3, false, 0, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, false, 2, cardSource: this.GetCardSource());
        }

        protected override string IfNotSoloMessage()
        {
            return "no cards can be played";
        }
    }
}