using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.NightMist;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.NightMist
{
    public class NightMistCombatReadyCharacterCardController : NightMistCharacterCardController
    {
        public NightMistCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
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
                SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "MasterOfMagic", "TomeOfElderMagic" });
            }
        }

    }
}