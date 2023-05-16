using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class SynchronizeStrikeCardController : CardController
    {
        public SynchronizeStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero && tt != this.TurnTaker), SelectionType.UsePower, (TurnTaker tt) => this.GameController.SelectAndUsePower(this.GameController.FindHeroTurnTakerController(tt.ToHero()), true, cardSource: this.GetCardSource()), 2, false, 0, cardSource: this.GetCardSource());
        }
    }
}