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
            this.AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardToPlay.DoKeywordsContain("orbital") && pca.WasCardPlayed && !pca.IsPutIntoPlay, (PlayCardAction pca) => this.GameController.DrawCards(this.DecisionMaker, 1, true, cardSource: this.GetCardSource()), TriggerType.DrawCard, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<PlayCardAction> pcaResult = new List<PlayCardAction>();

            coroutine = this.GameController.SelectHeroToPlayCard(this.DecisionMaker, true, storedResultsSelectCard: pcaResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}