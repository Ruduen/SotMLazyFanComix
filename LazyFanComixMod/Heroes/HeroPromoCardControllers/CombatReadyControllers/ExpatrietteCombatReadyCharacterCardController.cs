using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteCombatReadyCharacterCardController : ExpatrietteCharacterCardController
    {
        public ExpatrietteCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            this.AddStartOfTurnTrigger(
                (tt) => !this.IsPropertyTrue(SharedCombatReadyCharacter.SetupDone),
                (pca) => SharedCombatReadyCharacter.SetFlag(this),
                TriggerType.Hidden
            );
            if (!this.HasBeenSetToTrueThisGame(SharedCombatReadyCharacter.SetupDone))
            {
                SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "SpeedLoading", "Prejudice" });
            }
        }

    }
}