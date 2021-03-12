using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public class SoulshardOfMercuryCharacterCardController : SoulbinderSharedMultiCharacterCardController
    {
        public SoulshardOfMercuryCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.CardWithoutReplacements.IsFlipped)
            {
                this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DiscardToDrawResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DrawCard });
            }
        }

        private IEnumerator DiscardToDrawResponse(PhaseChangeAction arg)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine;
            // Discard card.
            coroutine = this.GameController.SelectAndDiscardCard(this.DecisionMaker, true, null, storedResults, SelectionType.DiscardCard, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidDiscardCards(storedResults))
            {
                // If you do, draw a card.
                coroutine = this.DrawCard(this.HeroTurnTaker, false, null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}