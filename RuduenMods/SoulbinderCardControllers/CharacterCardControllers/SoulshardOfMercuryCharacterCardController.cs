using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulshardOfMercuryCharacterCardController : SoulbinderSharedMultiCharacterCardController
    {

        public SoulshardOfMercuryCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, Phase.DrawCard, 1);
            }
        }

        public override IEnumerator Play()
        {
            // Increase phase count if appropriate, based on trigger.
            IEnumerator coroutine = this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == this.TurnTaker, Phase.DrawCard, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            // Remove normal trigger if appropriate, since normally this is only handled by the destruction/removal of the card. 
            IEnumerator coroutine;
            coroutine = this.ReducePhaseActionCountIfInPhase((TurnTaker tt) => tt == this.TurnTaker, Phase.DrawCard, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = base.BeforeFlipCardImmediateResponse(flip);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}