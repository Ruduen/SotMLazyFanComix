using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public class WastelandWandererCardController : CardController
    {
        private TokenPool _tokenPool;
        public WastelandWandererCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.IsEnvironment && dca.WasCardDestroyed, this.ResponseAction, TriggerType.AddTokensToPool, TriggerTiming.After);
            this.AddWhenDestroyedTrigger((DestroyCardAction dca) => this.OnDestroyedTrigger(), new TriggerType[] { TriggerType.Hidden, TriggerType.DealDamage });
        }

        private IEnumerator OnDestroyedTrigger()
        {
            IEnumerator coroutine;

            // Deal damage equal to the number of tokens.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), GetTokenPool().CurrentValue, DamageType.Fire, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Clear values for future uses. 
            coroutine = this.ResetTokenValue();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.ResetTokenValue();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int powerNumeral = this.GetPowerNumeral(0, 2);

            coroutine = this.GameController.AddTokensToPool(GetTokenPool(), powerNumeral, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        protected IEnumerator ResponseAction(DestroyCardAction dca)
        {
            IEnumerator coroutine = this.GameController.AddTokensToPool(GetTokenPool(), 1, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        public IEnumerator ResetTokenValue()
        {
            GetTokenPool().SetToInitialValue();
            yield break;
        }

        private TokenPool GetTokenPool()
        {
            if (_tokenPool == null)
            {
                _tokenPool = this.Card.FindTokenPool("WastelandWandererPool");
            }
            return _tokenPool;
        }
    }
}