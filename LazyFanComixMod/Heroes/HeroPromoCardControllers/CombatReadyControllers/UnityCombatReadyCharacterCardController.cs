using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Unity;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.Unity
{
    public class UnityCombatReadyCharacterCardController : UnityCharacterCardController
    {
        public UnityCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }
        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "VolatileParts", "PlatformBot" });
        }
    }
}