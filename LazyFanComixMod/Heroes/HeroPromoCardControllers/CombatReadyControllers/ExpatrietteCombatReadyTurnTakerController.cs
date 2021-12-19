using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public ExpatrietteCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "SpeedLoading", "Prejudice" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is ExpatrietteCombatReadyCharacterCardController)
                {
                    ExpatrietteCombatReadyCharacterCardController ccc = (ExpatrietteCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

    }
}