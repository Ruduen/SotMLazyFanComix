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
            this.AddTrigger<RemoveTokensFromPoolAction>(
                (RemoveTokensFromPoolAction rtfpa) => rtfpa.TokenPool == RitualPool && rtfpa.TokenPool.CurrentValue == 0, RitualCompleteInitialResponse, RitualTriggerTypes, TriggerTiming.After
            );
            this.AddWhenDestroyedTrigger((DestroyCardAction dc) => this.ResetTokenValue(), TriggerType.Hidden);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = ResetTokenValue();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.AddTokensToPool(RitualPool, 3, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator ResetTokenValue()
        {
            RitualPool.SetToInitialValue();
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int tokensToRemove = this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.IsTarget && c.Owner == this.TurnTaker).Count();

            coroutine = this.GameController.SendMessageAction("Removing " + tokensToRemove + " Ritual Token from " + this.Card.AlternateTitleOrTitle + ".", Priority.Low, cardSource: this.GetCardSource(), new Card[] { this.Card }, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.RemoveTokensFromPool(RitualPool, tokensToRemove, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RitualCompleteInitialResponse(RemoveTokensFromPoolAction rtfpa)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SendMessageAction(this.Card.AlternateTitleOrTitle + "'s last Ritual Token has been removed, so it will now activate!", Priority.Low, cardSource: this.GetCardSource(), null, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = RitualCompleteResponse();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected abstract IEnumerator RitualCompleteResponse();
    }
}