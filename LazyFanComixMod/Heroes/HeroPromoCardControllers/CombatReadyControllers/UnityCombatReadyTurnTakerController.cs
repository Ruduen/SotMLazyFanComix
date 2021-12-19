using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Unity
{
    public class UnityCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public UnityCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "VolatileParts", "PlatformBot" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is UnityCombatReadyCharacterCardController)
                {
                    UnityCombatReadyCharacterCardController ccc = (UnityCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

    }
}