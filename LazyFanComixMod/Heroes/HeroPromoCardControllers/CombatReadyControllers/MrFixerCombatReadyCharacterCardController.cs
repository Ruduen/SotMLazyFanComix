using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.MrFixer;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.MrFixer
{
    public class MrFixerCombatReadyCharacterCardController : MrFixerCharacterCardController
    {
        public MrFixerCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}