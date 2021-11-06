using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecallSharedDamageSelfCardController : CardController
    {
        public RecallSharedDamageSelfCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected IEnumerator DamageSelfMoveCard()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, CardsUnderSelf() * 2, DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, this.Card, this.CharacterCard.UnderLocation, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected int CardsUnderSelf()
        {
            return this.CharacterCard.UnderLocation.Cards.Count();
        }
    }
}