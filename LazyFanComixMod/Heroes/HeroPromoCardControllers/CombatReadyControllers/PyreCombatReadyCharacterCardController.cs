using Cauldron.Pyre;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.Pyre
{
    public class PyreCombatReadyCharacterCardController : PyreCharacterCardController
    {
        public PyreCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "CherenkovDrive", "ParticleCollider" });
        }

    }
}