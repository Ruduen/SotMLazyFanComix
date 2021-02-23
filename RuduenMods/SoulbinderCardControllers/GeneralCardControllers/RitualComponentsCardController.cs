using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class RitualComponentsCardController : CardController
    {

        public RitualComponentsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay.DoKeywordsContain("ritual") && cepa.CardEnteringPlay.FindTokenPool("RitualTokenPool") != null && cepa.CardEnteringPlay.FindTokenPool("RitualTokenPool").CurrentValue > 0 && cepa.IsSuccessful, RemoveTokenResponse, TriggerType.ModifyTokens, TriggerTiming.After);
        }

        private IEnumerator RemoveTokenResponse(CardEntersPlayAction cepa)
        {
            TokenPool ritualpool = cepa.CardEnteringPlay.FindTokenPool("RitualTokenPool");
            IEnumerator coroutine;

            coroutine = this.GameController.RemoveTokensFromPool(ritualpool, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.Owner == this.TurnTaker && c.IsTarget).Count() >= 3)
            {
                coroutine = this.GameController.RemoveTokensFromPool(ritualpool, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {

            IEnumerator coroutine;
            List<Card> actedTargets = new List<Card>();
            // Select target to deal damage to.
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 1) // Number of Tokens
            };

            // Each valid ritual loses a token. This needs to be done to handle scenarios where things are destroyed or played mid-way!
            SelectCardsDecision scdRituals = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0, SelectionType.RemoveTokens, null, false, null, true, true, false, () => NumRitualsToRemoveToken(actedTargets), null, null, null, this.GetCardSource());
            coroutine = this.GameController.SelectCardsAndDoAction(scdRituals, (SelectCardDecision scd) => this.RemoveTokenResponse(scd, actedTargets, numerals[0]), null, null, this.GetCardSource(), null, false, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }


        private IEnumerator RemoveTokenResponse(SelectCardDecision scd, List<Card> actedTargets, int numeral)
        {
            if (scd.SelectedCard != null)
            {
                actedTargets.Add(scd.SelectedCard);
                IEnumerator coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), numeral, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
        private int NumRitualsToRemoveToken(List<Card> actedTargets)
        {
            if (!this.TurnTaker.IsIncapacitatedOrOutOfGame)
            {
                int num = this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain("ritual") && c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0).Except(actedTargets).Count();
                return actedTargets.Count() + num;
            }
            return 0;
        }


    }
}