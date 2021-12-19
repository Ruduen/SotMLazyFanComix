using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.CombatReady;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.CaptainCosmic
{
    public class CaptainCosmicCombatReadyTurnTakerController : CombatReadySharedTurnTakerController
    {
        public CaptainCosmicCombatReadyTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        protected override string[] _setupCardIdentifiers
        {
            get
            {
                return new string[] { "SustainedInfluence", "DestructiveResponse" };
            }
        }

        protected override IEnumerator _drawUntilFour
        {
            get
            {
                if (this.CharacterCardController is CaptainCosmicCombatReadyCharacterCardController)
                {
                    CaptainCosmicCombatReadyCharacterCardController ccc = (CaptainCosmicCombatReadyCharacterCardController)this.CharacterCardController;
                    return ccc.DrawUntilFour();
                }
                return null;
            }
        }

        public override IEnumerator StartGame()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.MoveCard(this, this.TurnTaker.GetCardByIdentifier("CosmicWeapon"), this.CharacterCard.NextToLocation, false, true, false, cardSource: new CardSource(this.CharacterCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = base.StartGame();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}