using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Linq;

namespace LazyFanComix.MrFixer
{
    public class MrFixerCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public MrFixerCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "PipeWrench", "Harmony" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is MrFixerCombatReadyCharacterCardController)
                {
                    MrFixerCombatReadyCharacterCardController ccc = (MrFixerCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

    }
}