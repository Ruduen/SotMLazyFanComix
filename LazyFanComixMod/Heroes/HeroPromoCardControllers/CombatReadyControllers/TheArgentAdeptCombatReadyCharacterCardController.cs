using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.TheArgentAdept;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.TheArgentAdept
{
    public class TheArgentAdeptCombatReadyCharacterCardController : TheArgentAdeptCharacterCardController
    {
        public TheArgentAdeptCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "AlacritousSubdominant", "ScherzoOfFrostAndFlame" });
        }
    }
}