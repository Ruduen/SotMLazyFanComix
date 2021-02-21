using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public class FleshOfMercuryCharacterCardController : SynthesistSharedMultiCharacterCardController
    {
        public FleshOfMercuryCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override void AddIndividualTrigger()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.PlayCard, 1);
        }

        public override IEnumerator Play()
        {
            // Increase phase count if appropriate, based on trigger.
            IEnumerator coroutine = this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == this.HeroTurnTaker, Phase.PlayCard, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == this.HeroTurnTaker;
        }
    }
}