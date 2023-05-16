using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.CaptainCosmic;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using System.Collections;

namespace LazyFanComix.CaptainCosmic
{
    public class CaptainCosmicCombatReadyCharacterCardController : CaptainCosmicCharacterCardController
    {
        public CaptainCosmicCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.MoveCard(this.DecisionMaker, SharedCombatReadyCharacter.GetPreferringDeck(this.TurnTaker, "CosmicWeapon"), this.CharacterCard.NextToLocation, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = SharedCombatReadyCharacter.InitialSetupPutInPlay(this, new string[] { "DestructiveResponse", "SustainedInfluence" });
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}