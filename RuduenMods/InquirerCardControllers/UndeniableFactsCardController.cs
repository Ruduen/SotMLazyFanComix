using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace RuduenWorkshop.Inquirer
{
    public class UndeniableFactsCardController : CardController
    {
        // TODO: TEST!
        public UndeniableFactsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, Phase.DrawCard, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 2);

            // Play 2 Distortion cards.
            IEnumerator coroutine = this.SelectAndPlayCardsFromHand(this.DecisionMaker, powerNumeral, false, new int?(0), new LinqCardCriteria((Card c) => c.IsDistortion, "distortion", true));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}