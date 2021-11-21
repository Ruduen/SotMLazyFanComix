using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class HoldingTheHighGroundCardController : CardController
    {
        public HoldingTheHighGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartTurnDamage, TriggerType.DealDamage);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndTurnGainHP, TriggerType.GainHP);
        }

        private IEnumerator StartTurnDamage(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, GetFigureheadWithMostCardsUnder());

            // Selected villain damages and, if legal, discovers a matching card.
            coroutine = this.GameController.SelectCardAndDoAction(scd, DealDamageDelegate);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DealDamageDelegate(SelectCardDecision scd)
        {
            Card figurehead = scd.SelectedCard;
            IEnumerator coroutine;
            if (figurehead != null)
            {
                // Damaging aspects. 
                coroutine = this.GameController.DealDamage(this.DecisionMaker, figurehead, (Card c) => c != figurehead && c.DoKeywordsContain("figurehead"), 2, DamageType.Psychic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DealDamage(this.DecisionMaker, figurehead, (Card c) => !c.DoKeywordsContain("figurehead") && !c.DoKeywordsContain(figurehead.GetKeywords()), 2, DamageType.Psychic, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator EndTurnGainHP(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            SelectCardDecision scd = new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.DealDamage, GetFigureheadWithMostCardsUnder());

            // Selected villain damages and, if legal, discovers a matching card.
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

        private IEnumerable<Card> GetFigureheadWithMostCardsUnder()
        {
            // Look over all figureheads for the one with the most cards. There doesn't appear to be a valid method of using TargetInfo for this, so this is done manually.
            IEnumerable<Card> figureheads = this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsVillain && c.DoKeywordsContain("figurehead"));
            int maxCards = figureheads.Select((Card c) => c.UnderLocation.Cards.Count()).Max();
            return figureheads.Where((Card c) => c.UnderLocation.Cards.Count() == maxCards);
        }
    }
}
