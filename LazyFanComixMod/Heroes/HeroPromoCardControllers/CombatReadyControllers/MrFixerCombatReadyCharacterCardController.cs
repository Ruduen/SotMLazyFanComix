using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.MrFixer;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.MrFixer
{
    public class MrFixerCombatReadyCharacterCardController : MrFixerCharacterCardController
    {
        public MrFixerCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger(
                (tt) => !this.IsPropertyTrue(SharedCombatReadyCharacter.SetupDone),
                (pca) => SharedCombatReadyCharacter.SetFlag(this),
                TriggerType.Hidden
            );
            if (!this.HasBeenSetToTrueThisGame(SharedCombatReadyCharacter.SetupDone))
            {
                SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "PipeWrench", "Harmony" });
            }
        }
    }
}