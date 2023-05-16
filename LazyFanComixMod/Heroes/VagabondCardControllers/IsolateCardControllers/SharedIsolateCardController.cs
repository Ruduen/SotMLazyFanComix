using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace LazyFanComix.Vagabond
{
    public abstract class SharedIsolateCardController : CardController
    {
        // TODO: Needs AskIfActionCanBePerformed?
        public SharedIsolateCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public abstract void AddUniqueTriggers();

        public override void AddTriggers()
        {
            this.AddUniqueTriggers();
            this.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && md.CardSource.Card.Owner.IsHero, new Func<MakeDecisionsAction, IEnumerator>(this.RemoveDecisionsFromMakeDecisionsResponse), TriggerType.RemoveDecision, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardCriteria: new LinqCardCriteria((Card c) => !c.DoKeywordsContain("solo"), "non-solo"), cardSource: this.GetCardSource()), TriggerType.PlayCard);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource()), TriggerType.DestroySelf);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner != this.TurnTaker && d.HeroTurnTakerController.TurnTaker == this.TurnTaker);
            md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner == this.TurnTaker && d.HeroTurnTakerController.TurnTaker != this.TurnTaker);
            yield return this.DoNothing();
            yield break;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            return this.AskIfTurnTakerIsVisibleToCardSource(card.Owner, cardSource);
        }

        public override bool? AskIfTurnTakerIsVisibleToCardSource(TurnTaker tt, CardSource cardSource)
        {
            if (cardSource == null || !cardSource.Card.IsHero || !tt.IsHero)
            {
                return true;
            }
            if (cardSource.Card.Owner == this.TurnTaker)
            {
                return tt == this.TurnTaker;
            }
            return tt != this.TurnTaker;
        }
    }
}