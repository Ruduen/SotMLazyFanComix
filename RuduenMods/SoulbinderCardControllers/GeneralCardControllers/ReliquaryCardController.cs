using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class ReliquaryCardController : CardController
    {

        public ReliquaryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>(
                (DestroyCardAction dca)=> dca.CardToDestroy.Card.Owner == this.TurnTaker && dca.CardToDestroy.Card.IsCharacter,
                PreventDestructionResponse,
                new TriggerType[] {TriggerType.CancelAction, TriggerType.GainHP, TriggerType.DestroyCard, TriggerType.DestroySelf},
                TriggerTiming.Before
            );
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Move a card from under the instructions card into play.
            Card card = this.TurnTaker.FindCard("SoulbinderCharacter", false);
            if (card != null)
            {
                coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, card.UnderLocation, 1, 1, new LinqCardCriteria(), new List<MoveCardDestination>() { new MoveCardDestination(this.TurnTaker.PlayArea) }, true, optional: true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator PreventDestructionResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> yncdResult = new List<YesNoCardDecision>();

            coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.PreventDestruction, dca.CardToDestroy.Card, storedResults: yncdResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if(yncdResult.Count>0 && yncdResult.FirstOrDefault().Answer == true)
            {
                coroutine = this.GameController.CancelAction(dca, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.SetHP(dca.CardToDestroy.Card, 6, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}