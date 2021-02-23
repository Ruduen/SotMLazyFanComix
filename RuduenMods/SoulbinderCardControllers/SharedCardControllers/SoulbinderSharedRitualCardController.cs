using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public abstract class SoulbinderSharedRitualCardController : CardController
    {
        private TokenPool _ritualPool;

        private TokenPool RitualPool
        {
            get
            {
                if (_ritualPool == null)
                {
                    _ritualPool = this.Card.FindTokenPool("RitualTokenPool");
                }
                return _ritualPool;
            }
        }

        protected abstract TriggerType[] RitualTriggerTypes { get; }
        public SoulbinderSharedRitualCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, RemoveTokenResponse, TriggerType.ModifyTokens);
            this.AddTrigger<RemoveTokensFromPoolAction>(
                (RemoveTokensFromPoolAction rtfpa) => rtfpa.TokenPool == RitualPool && rtfpa.TokenPool.CurrentValue == 0, RitualCompleteInitialResponse, RitualTriggerTypes, TriggerTiming.After
            );
        }

        private IEnumerator RemoveTokenResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.GameController.RemoveTokensFromPool(RitualPool, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


        private IEnumerator RitualCompleteInitialResponse(RemoveTokensFromPoolAction rtfpa)
        {
            IEnumerator coroutine;
            coroutine = RitualCompleteResponse();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected abstract IEnumerator RitualCompleteResponse();

    }
}