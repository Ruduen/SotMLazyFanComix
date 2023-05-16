using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.NightMist;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.NightMist
{
    public class NightMistCombatReadyCharacterCardController : NightMistCharacterCardController
    {
        public NightMistCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "MasterOfMagic", "TomeOfElderMagic" });
        }
    }
}