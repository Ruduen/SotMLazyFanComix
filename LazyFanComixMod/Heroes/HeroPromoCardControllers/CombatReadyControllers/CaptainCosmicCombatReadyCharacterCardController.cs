using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.CaptainCosmic;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.CaptainCosmic
{
    public class CaptainCosmicCombatReadyCharacterCardController : CaptainCosmicCharacterCardController
    {
        public CaptainCosmicCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
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
                InitialSetup();
            }
        }

        private void InitialSetup()
        {
            this.TurnTaker.MoveCard(SharedCombatReadyCharacter.GetPreferringDeck(this.TurnTaker, "CosmicWeapon"), this.CharacterCard.NextToLocation);
            SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "DestructiveResponse", "SustainedInfluence" });
        }

    }
}