using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Trailblazer
{
    public class ClimbingHarnessCardController : CardController
    {
        private const string _FirstDamageDealtThisTurn = "FirstDamageDealtThisTurn";

        public ClimbingHarnessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstDamageDealtThisTurn, this.CharacterCard.Title + " has already dealt damage this turn.", this.CharacterCard.Title + " has not yet dealt damage this turn.");
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource.Card != null && dda.DamageSource.Card == this.CharacterCard && !this.IsPropertyTrue(_FirstDamageDealtThisTurn), TrackAndIncreaseResponse, TriggerType.IncreaseDamage, TriggerTiming.Before);
            this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstDamageDealtThisTurn), TriggerType.Hidden);
        }

        private IEnumerator TrackAndIncreaseResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;

            this.SetCardPropertyToTrueIfRealAction(_FirstDamageDealtThisTurn);

            if (this.GameController.ActiveTurnTaker == this.TurnTaker || this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsPosition).Count() > 0)
            {
                coroutine = this.GameController.IncreaseDamage(dda, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}