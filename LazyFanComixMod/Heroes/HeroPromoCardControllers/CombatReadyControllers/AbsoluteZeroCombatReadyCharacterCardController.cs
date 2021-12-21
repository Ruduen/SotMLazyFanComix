using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.AbsoluteZero;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using LazyFanComix.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AbsoluteZero
{
    public class AbsoluteZeroCombatReadyCharacterCardController : AbsoluteZeroCharacterCardController
    {
        public AbsoluteZeroCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "IsothermicTransducer", "GlacialStructure" });
        }
    }
}