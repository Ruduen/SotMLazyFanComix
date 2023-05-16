using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace LazyFanComix.TheTurfWar
{
    public class SezCardController : TheTurfWarSharedCharacterCardController
    {
        public SezCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            IEnumerator coroutine = this.DealDamageToHighestHP(this.Card, 1, (Card c) => !c.IsThug, (Card c) => 3, DamageType.Projectile, numberOfTargets: () => this.H - 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyCardsResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            if (this.Card.UnderLocation.Cards.Count() >= 1)
            {
                coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsThug, 3, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.GameController.FindEnvironmentTurnTakerController());
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