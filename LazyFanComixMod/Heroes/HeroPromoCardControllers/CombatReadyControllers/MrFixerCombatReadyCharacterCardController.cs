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
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }
        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "PipeWrench", "Harmony" });
        }

    }
}