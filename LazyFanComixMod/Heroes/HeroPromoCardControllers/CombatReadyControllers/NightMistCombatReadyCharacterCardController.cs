using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.NightMist;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.NightMist
{
    public class NightMistCombatReadyCharacterCardController : NightMistCharacterCardController
    {
        public NightMistCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}