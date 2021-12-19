using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Unity;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Unity
{
    public class UnityCombatReadyCharacterCardController : UnityCharacterCardController
    {
        public UnityCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator DrawUntilFour()
        {
            return this.DrawCardsUntilHandSizeReached(this.DecisionMaker, 4);
        }

    }
}