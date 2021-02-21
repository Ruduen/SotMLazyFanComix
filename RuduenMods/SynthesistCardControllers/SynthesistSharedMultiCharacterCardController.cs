using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public abstract class SynthesistSharedMultiCharacterCardController : HeroCharacterCardController
    {
        public SynthesistSharedMultiCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                IEnumerator coroutine = this.DoAction(new RemoveTargetAction(base.GameController, base.CardWithoutReplacements, true));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            yield break;
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            // Also do default trigger handling. 
            this.RemoveAllTriggers();
            this.AddSideTriggers();
            yield break;
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddIndividualTrigger();
            }
        }

        protected abstract void AddIndividualTrigger();
    }
}