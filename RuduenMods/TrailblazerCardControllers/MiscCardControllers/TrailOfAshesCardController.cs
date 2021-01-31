using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class TrailOfAshesCardController : CardController
    {
        public TrailOfAshesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Deal each non-Hero target 1 damage.
            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.HeroTurnTaker.CharacterCard, new Func<Card, bool>((Card c) => !c.IsHero), 2, DamageType.Fire, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, 2, DamageType.Fire, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If there is a position in play...
            if(this.GameController.FindCardsWhere((Card c) => c.IsPosition && c.IsInPlay).Count() > 0)
            {
                // Draw 2 cards.
                coroutine = this.GameController.DrawCards(this.DecisionMaker, 2, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Destroy a position.
                coroutine = this.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsPosition, "position"), false, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}