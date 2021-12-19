using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Bunker;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Bunker
{
    public class BunkerCombatReadyCharacterCardController : BunkerCharacterCardController
    {
        public BunkerCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}