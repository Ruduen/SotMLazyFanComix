using Cauldron.Necro;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.Necro
{
    public class NecroCombatReadyCharacterCardController : NecroCharacterCardController
    {
        public NecroCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            // Tainted Blood has trigger issues, so I can't use it here.
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "CorpseExplosion", "NecroZombie" });
        }
    }
}