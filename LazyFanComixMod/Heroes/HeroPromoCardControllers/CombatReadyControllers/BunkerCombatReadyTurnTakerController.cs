using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Bunker
{
    public class BunkerCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public BunkerCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "FlakCannon", "AuxiliaryPowerSource" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is BunkerCombatReadyCharacterCardController)
                {
                    BunkerCombatReadyCharacterCardController ccc = (BunkerCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

    }
}