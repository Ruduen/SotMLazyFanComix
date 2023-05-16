using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class CalinCardController : TheTurfWarSharedCharacterCardController
    {
        public CalinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override ITrigger AddUniqueActiveTrigger()
        {
            return this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
        }

        protected override ITrigger AddUniqueIncapacitatedTrigger()
        {
            return this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DestroyCardsResponse), TriggerType.DestroyCard, null, false);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, this.Card, (Card c) => true, 2, DamageType.Infernal, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyCardsResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            if (this.Card.UnderLocation.Cards.Count() >= 3)
            {
                coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), 3, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.SendMessageAction(this.Card.Title + " re-enters play.", Priority.Medium, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.SetHP(this.Card, 15, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.MoveCards(this.TurnTakerController, this.FindEnvironment().TurnTaker.Deck.GetTopCards(1), this.Card.UnderLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}