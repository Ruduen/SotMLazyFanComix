using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class SpiritialResonanceCardController : SoulbinderSharedYourTargetDamageCardController
    {
        public SpiritialResonanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> targetList = new List<Card>();

            // Select target.
            coroutine = this.SelectYourTargetToDealDamage(targetList, 1, DamageType.Infernal);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (targetList.Count > 0)
            {
                // That target deals itself 1 damage.
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, targetList.FirstOrDefault()), targetList.FirstOrDefault(), 1, DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select type.
            List<Function> list = new List<Function>
                {
                    new Function(this.DecisionMaker, "reveal a ritual", SelectionType.RevealTopCardOfDeck,
                    () => this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.HeroTurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ritual"), "ritual"), 1, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards, shuffleReturnedCards: true),
                    this.HeroTurnTaker.Deck.Cards.Count() > 0),
                     new Function(this.DecisionMaker, "reveal a soulsplinter", SelectionType.RevealTopCardOfDeck,
                    () => this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.HeroTurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("soulsplinter"), "soulsplinter"), 1, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards, shuffleReturnedCards: true),
                    this.HeroTurnTaker.Deck.Cards.Count() > 0),
                };

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " does not have any cards in their deck, so" + this.Card.AlternateTitleOrTitle + " has no effect.", null, this.GetCardSource());
            coroutine = this.GameController.SelectAndPerformFunction(selectFunction, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}