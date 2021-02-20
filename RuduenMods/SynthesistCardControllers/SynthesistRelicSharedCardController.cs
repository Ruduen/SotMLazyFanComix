using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Synthesist
{
    public abstract class SynthesistRelicSharedCardController : HeroCharacterCardController
    {
        public SynthesistRelicSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddIndividualTrigger();
            }
        }
        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                RemoveTargetAction action = new RemoveTargetAction(this.GameController, this.CardWithoutReplacements, true);
                IEnumerator coroutine;
                    coroutine = this.GameController.DoAction(action);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // If appropriate, remove card from game. 

                coroutine = this.GameController.MoveCard(this.DecisionMaker, this.CardWithoutReplacements, this.HeroTurnTaker.OutOfGame, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                this.RemoveAllTriggers();
            }
            yield break;
        }

        protected abstract void AddIndividualTrigger();
    }
}