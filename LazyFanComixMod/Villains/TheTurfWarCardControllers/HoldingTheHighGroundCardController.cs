using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class HoldingTheHighGroundCardController : CardController
    {
        public HoldingTheHighGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnDamage, TriggerType.DealDamage);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndTurnGainHP, TriggerType.GainHP);
        }

        private IEnumerator StartOfTurnDamage(PhaseChangeAction arg)
        {
            IEnumerator coroutine;
            // Look over all figureheads for the ones with the most cards. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));

            Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
            int cardCountMax = -1;
            foreach (Card c in figureheads)
            {
                int cardCount = c.UnderLocation.Cards.Count();
                cardCounts.Add(c, cardCount);
                cardCountMax = Math.Max(cardCountMax, cardCount);
            }

            figureheads = cardCounts.Where((KeyValuePair<Card, int> pair) => pair.Value == cardCountMax).Select((KeyValuePair<Card, int> pair) => pair.Key);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, figureheads, cardSource: this.GetCardSource());

            // Selected villain gains HP.
            coroutine = this.GameController.SelectCardAndDoAction(scd, DealDamageDelegate);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DealDamageDelegate(SelectCardDecision scd)
        {
            Card figurehead = scd.SelectedCard;
            IEnumerator coroutine;
            if (figurehead != null)
            {
                coroutine = this.GameController.DealDamage(this.DecisionMaker, figurehead, (Card c) => !c.IsVillain, 2, DamageType.Psychic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator EndTurnGainHP(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            // Look over all figureheads for the ones with the most cards. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));

            Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
            int cardCountMax = -1;
            foreach (Card c in figureheads)
            {
                int cardCount = c.UnderLocation.Cards.Count();
                cardCounts.Add(c, cardCount);
                cardCountMax = Math.Max(cardCountMax, cardCount);
            }

            figureheads = cardCounts.Where((KeyValuePair<Card, int> pair) => pair.Value == cardCountMax).Select((KeyValuePair<Card, int> pair) => pair.Key);
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.GainHP, figureheads, cardSource: this.GetCardSource());

            // Selected villain gains HP.
            coroutine = this.GameController.SelectCardAndDoAction(scd, GainHPDelegate);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator GainHPDelegate(SelectCardDecision scd)
        {
            Card figurehead = scd.SelectedCard;
            IEnumerator coroutine;
            if (figurehead != null)
            {
                coroutine = this.GameController.GainHP(figurehead, 5, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}