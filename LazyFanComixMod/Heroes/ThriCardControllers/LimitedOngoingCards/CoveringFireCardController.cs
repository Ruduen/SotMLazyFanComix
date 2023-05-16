using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class CoveringFireCardController : CardController
    {
        public CoveringFireCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddPreventDamageTrigger((DealDamageAction dda) => dda.DamageSource.Card == this.CharacterCard && dda.Target.IsHeroCharacterCard && dda.DamageType == DamageType.Projectile, MoveTargetCardResponse, new TriggerType[] { TriggerType.MoveCard });
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => !dda.DamageSource.Card.IsHero && dda.Target.IsHeroCharacterCard && this.Card.UnderLocation.Cards.Select((Card c) => c.Owner).Contains(dda.Target.Owner), DestroyCardsToReduceDamageResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.ReduceDamage }, TriggerTiming.Before, orderMatters: true);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0,1)
            };
            return this.GameController.SelectAndUsePower(this.DecisionMaker, true, numberOfPowers: powerNumerals[0], cardSource: this.GetCardSource());
        }

        private IEnumerator MoveTargetCardResponse(DealDamageAction dda)
        {
            if (dda?.Target?.Owner?.Deck?.BottomCard != null)
            {
                return this.GameController.MoveCard(this.DecisionMaker, dda.Target.Owner.Deck.BottomCard, this.Card.UnderLocation, playCardIfMovingToPlayArea: false, showMessage: true, cardSource: this.GetCardSource());
            }
            return this.GameController.SendMessageAction("The deck has no cards to move.", Priority.Low, this.GetCardSource(), showCardSource: true);
        }

        private IEnumerator DestroyCardsToReduceDamageResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            int count;
            List<DestroyCardAction> dcas = new List<DestroyCardAction>();

            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation && c.Owner == dda.Target.Owner), storedResults: dcas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            count = dcas.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Count();

            coroutine = this.GameController.ReduceDamage(dda, count * 2, null, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}