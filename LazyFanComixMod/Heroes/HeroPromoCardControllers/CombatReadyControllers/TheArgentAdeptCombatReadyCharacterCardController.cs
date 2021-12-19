using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.TheArgentAdept;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.TheArgentAdept
{
    public class TheArgentAdeptCombatReadyCharacterCardController : TheArgentAdeptCharacterCardController
    {
        public TheArgentAdeptCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
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
                SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "AlacritousSubdominant", "ScherzoOfFrostAndFlame" });
            }
        }

    }
}