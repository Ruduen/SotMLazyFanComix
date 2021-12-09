using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.SkyScraper;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.SkyScraper
{
    public abstract class SkyScraperQuickShiftSharedCharacterCardController : SkyScraperCharacterCardController
    {
        protected abstract string cardKeyword();
        protected abstract string charKeyword();
        protected List<string> otherCharKeywords()
        {
            List<string> keywords = new List<string>() { "Tiny", "Normal", "Huge" };
            keywords.Remove(charKeyword());
            return keywords;
        }
        public SkyScraperQuickShiftSharedCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // Draw a card or play a <size> card.
            List<Function> choices;
            choices = new List<Function>() {
                new Function(this.DecisionMaker, "Play a " + cardKeyword() + " card", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, false, cardCriteria: new LinqCardCriteria((Card c) => c != null && c.DoKeywordsContain(cardKeyword())), cardSource: this.GetCardSource()), this.DecisionMaker?.HeroTurnTaker?.Hand?.Cards != null && this.CanPlayCardsFromHand(this.DecisionMaker) && this.DecisionMaker.HeroTurnTaker.Hand.Cards.Any((Card c)=>c.DoKeywordsContain(cardKeyword())), this.TurnTaker.Name + " cannot draw any cards, so they must play a " + cardKeyword() + " card."),
                new Function(this.DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource()), this.DecisionMaker != null && this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any " + cardKeyword() + " cards, so they must draw a card.")
            };

            coroutine = this.GameController.SelectAndPerformFunction(new SelectFunctionDecision(this.GameController, this.DecisionMaker, choices, true, null, this.TurnTaker.Name + " cannot draw any cards or play any " + cardKeyword() + " cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource()));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select and switch sizes.            
            choices = new List<Function>();
            foreach (string keyword in otherCharKeywords())
            {
                Card c = this.TurnTaker.FindCard("SkyScraper" + keyword + "Character");
                if (c != null)
                {
                    choices.Add(new Function(this.DecisionMaker, "Switch to your " + keyword + " character car", SelectionType.SwitchToHero, () => this.GameController.SwitchCards(this.Card, c, cardSource: this.GetCardSource())));
                }
            }

            coroutine = this.GameController.SelectAndPerformFunction(
                new SelectFunctionDecision(this.GameController, this.DecisionMaker, choices, false, null, this.TurnTaker.Name + " is unable to switch to another character card.", cardSource: this.GetCardSource())
                );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}