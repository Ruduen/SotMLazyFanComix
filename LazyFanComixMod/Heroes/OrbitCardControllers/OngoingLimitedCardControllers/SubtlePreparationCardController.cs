using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Orbit
{
    public class SubtlePreparationCardController : CardController
    {
        public SubtlePreparationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddCannotDealDamageTrigger((Card c) => c == this.CharacterCard);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource()), TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.DrawCards(this.DecisionMaker, 2, true, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            return this.DrawACardOrPlayACard(this.DecisionMaker, false);
        }
    }
}