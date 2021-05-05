using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecallCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public RecallCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            list.Add(new Function(this.DecisionMaker, "Play a Paradox", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker,1,false, cardCriteria: new LinqCardCriteria((Card c)=>c.DoKeywordsContain("paradox") && !this.GameController.IsLimitedAndInPlay(Card)), cardSource: this.GetCardSource()),
                this.HeroTurnTaker.Hand.Cards.Where((Card c)=>c.DoKeywordsContain("paradox")).Any(), this.TurnTaker.Name + " cannot draw any cards, so they must play a Paradox."));
            list.Add(new Function(this.DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any Paradoxes, so they must draw a card."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw any cards or play any Paradoxes, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace Incap with something more unique!
    }
}