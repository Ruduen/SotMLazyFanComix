using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

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
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), TriggerType.DrawCard, null, false);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
        {
            // If there are no distortions, draw.
            if (this.FindCardsWhere((Card c) => c.IsInPlay && c.IsDistortion).Count() == 0)
            {
                IEnumerator coroutine = this.GameController.DrawCard(this.HeroTurnTaker, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
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