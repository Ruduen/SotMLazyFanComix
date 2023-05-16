using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace LazyFanComix.Vagabond
{
    public class FeignDeathCardController : CardController
    {
        public FeignDeathCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card == this.CharacterCard && !this.GameController.IsCardIndestructible(dca.CardToDestroy.Card), new Func<DestroyCardAction, IEnumerator>(this.RestoreAndDestroyResponse), new TriggerType[]
            {
                TriggerType.CancelAction, TriggerType.GainHP
            }, TriggerTiming.Before);
            this.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.SelfDestroyResponse), new TriggerType[] {
                TriggerType.PlayCard,
                TriggerType.ShuffleDeck,
                TriggerType.RemoveFromGame
            });
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => !c.IsHero, 2, DamageType.Fire, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RestoreAndDestroyResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;

            coroutine = this.CancelAction(dca, true, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SetHP(this.CharacterCard, 7, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SelfDestroyResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;

            // Search Deck and play card, and shuffle.
            coroutine = this.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, this.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("solo"), "solo"), new MoveCardDestination[] { new MoveCardDestination(this.TurnTaker.PlayArea) }, false, true, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Remove this from the game.
            coroutine = this.GameController.MoveCard(this.DecisionMaker, this.Card, this.TurnTaker.OutOfGame, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}