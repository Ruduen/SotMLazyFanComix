using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class RedesignateCardController : CardController
    {
        public RedesignateCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Function> functionList;
            SelectFunctionDecision sfd;

            coroutine = this.GameController.SelectAndReturnCards(this.DecisionMaker, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("orbital"), "orbital"), true, false, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            functionList = new List<Function>(){
            new Function(this.HeroTurnTakerController, "Play up to 3 Orbital cards", SelectionType.PlayCard,
                () => this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 3, false, 0, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("orbital"), "orbital"), cardSource: this.GetCardSource()),
                this.CanPlayCardsFromHand(this.HeroTurnTakerController),
                this.TurnTaker.Name + " cannot draw any cards, so they must play up to 3 Orbital cards."),
            new Function(this.HeroTurnTakerController, "Draw 3 cards", SelectionType.DrawCard, ()=>this.GameController.DrawCards(this.DecisionMaker,3,cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot play any cards, so they must draw 3 cards.")};

            sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, functionList, false, null, this.TurnTaker.Name + " cannot draw any cards or play any cards.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }

    }
}