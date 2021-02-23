using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class RepeatedRitesCardController : CardController
    {

        public RepeatedRitesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>(
                (DestroyCardAction dca) => dca.CardToDestroy.Card.IsInPlayAndNotUnderCard && dca.CardToDestroy.Card.DoKeywordsContain("ritual"),
                PreventDestructionResponse,
                new TriggerType[] { TriggerType.CancelAction, TriggerType.GainHP, TriggerType.DestroyCard, TriggerType.DestroySelf },
                TriggerTiming.Before
            );
        }

        private IEnumerator PreventDestructionResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> yncdResult = new List<YesNoCardDecision>();

            coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.PreventDestruction, dca.CardToDestroy.Card, storedResults: yncdResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (yncdResult.Count > 0 && yncdResult.FirstOrDefault().Answer == true)
            {
                TokenPool ritualPool = dca.CardToDestroy.Card.FindTokenPool("RitualTokenPool");
                coroutine = this.GameController.CancelAction(dca, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Only add tokens if the pool is valid, in case of some weird crossover. 
                if (ritualPool != null)
                {
                    coroutine = this.GameController.AddTokensToPool(ritualPool, 2, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }

                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}