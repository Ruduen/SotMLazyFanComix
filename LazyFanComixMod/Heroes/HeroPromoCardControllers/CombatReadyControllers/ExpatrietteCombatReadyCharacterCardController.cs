using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteCombatReadyCharacterCardController : ExpatrietteCharacterCardController
    {
        public ExpatrietteCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}