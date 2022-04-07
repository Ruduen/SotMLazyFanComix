using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class ObjectsInMotionCardController : CardController
    {
        public ObjectsInMotionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card.IsTarget && dda.DamageSource.Card != this.CharacterCard && this.GameController.ActiveTurnTaker == this.TurnTaker, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<PlayCardAction> pcaResult = new List<PlayCardAction>();

            coroutine = this.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectHeroToPlayCard(this.DecisionMaker, true, storedResultsSelectCard: pcaResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (pcaResult.Any((PlayCardAction pca) => pca.WasCardPlayed))
            {
                coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}