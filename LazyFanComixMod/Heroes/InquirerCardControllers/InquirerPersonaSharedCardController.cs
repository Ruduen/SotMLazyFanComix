﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Inquirer
{
    // Token: 0x0200054D RID: 1357
    public abstract class InquirerPersonaSharedCardController : CardController
    {
        // Token: 0x060028EC RID: 10476 RVA: 0x00023B10 File Offset: 0x00021D10
        public InquirerPersonaSharedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        // Token: 0x060028ED RID: 10477 RVA: 0x00066649 File Offset: 0x00064849
        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.MoveOrDestroyResponse), new TriggerType[]
            {
                TriggerType.MoveCard,
                TriggerType.DestroySelf
            }, null, false);
        }

        // TODO: Change to Select Cards and Move if appropriate.
        // TODO: Validate if DecisionMaker should be this.HeroTurnTakerController, or if this.HeroTurnTakerController works well.
        private IEnumerator MoveOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine;
            // If enough cards exist
            if (this.HeroTurnTaker.Trash.Cards.Count<Card>() >= 3)
            {
                // Ask if we should move the top two cards of the trash to the bottom of the deck for things.
                YesNoAmountDecision yesNoDecision = new YesNoAmountDecision(this.GameController, this.HeroTurnTakerController, SelectionType.MoveCard, 3, associatedCards: new List<Card> { this.Card }, cardSource: this.GetCardSource());
                coroutine = this.GameController.MakeDecisionAction(yesNoDecision);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (this.DidPlayerAnswerYes(yesNoDecision))
                {
                    List<MoveCardAction> storedResults = new List<MoveCardAction>();
                    // Move the top three cards.
                    coroutine = this.GameController.MoveCards(this.HeroTurnTakerController, this.HeroTurnTaker.Trash.GetTopCards(3), this.HeroTurnTaker.Deck, true, storedResultsAction: storedResults, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    if (storedResults.Count != 3 || storedResults.Any((MoveCardAction mca) => !mca.WasCardMoved))
                    {
                        // Failed Movement - destroy.
                        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, false, null, null, null, null, null, null, null, null, this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }
                else
                {
                    // No movement - destroy.
                    coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, false, null, null, null, null, null, null, null, null, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                // Not enough cards - automatically destroy.
                // TODO: Add message if appropriate.
                coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, false, null, null, null, null, null, null, null, null, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}