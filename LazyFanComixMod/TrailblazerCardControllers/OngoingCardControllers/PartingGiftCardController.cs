using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Trailblazer
{
    public class PartingGiftCardController : CardController
    {
        public PartingGiftCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.IsPosition && dca.WasCardDestroyed, this.ResponseAction, TriggerType.DealDamage, TriggerTiming.After);
        }

        protected IEnumerator ResponseAction(DestroyCardAction dca)
        {
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Fire, 2, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}