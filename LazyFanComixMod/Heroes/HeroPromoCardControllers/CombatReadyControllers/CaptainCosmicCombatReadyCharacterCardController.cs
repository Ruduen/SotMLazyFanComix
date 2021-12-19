using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.CaptainCosmic;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.CaptainCosmic
{
    public class CaptainCosmicCombatReadyCharacterCardController : CaptainCosmicCharacterCardController
    {
        public CaptainCosmicCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}