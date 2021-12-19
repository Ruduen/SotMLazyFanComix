using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Linq;

namespace LazyFanComix.NightMist
{
    public class NightMistCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public NightMistCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "MasterOfMagic", "TomeOfElderMagic" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is NightMistCombatReadyCharacterCardController)
                {
                    NightMistCombatReadyCharacterCardController ccc = (NightMistCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

    }
}