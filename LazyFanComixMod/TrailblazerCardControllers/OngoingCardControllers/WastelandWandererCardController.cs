using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Trailblazer
{
    public class WastelandWandererCardController : CardController
    {

        public WastelandWandererCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.IsEnvironment && dca.WasCardDestroyed, this.ResponseAction, new TriggerType[] { TriggerType.MoveCard, TriggerType.ChangePostDestroyDestination }, TriggerTiming.After);
            this.AddBeforeLeavesPlayActions(ReturnCardsToOwnersTrashResponse);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int[] numerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3),
                this.GetPowerNumeral(2, 1)
            };


            // Trigger to increase damage dealt by 1 for each card under the card.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => DamageBoost(numerals[2]));

            // Deal 1 target 3 fire damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), numerals[1], DamageType.Fire, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private int DamageBoost(int numeral)
        {
            return this.Card.UnderLocation.NumberOfCards * numeral;
        }

        private IEnumerator ResponseAction(DestroyCardAction dca)
        {
            List<YesNoCardDecision> yncdResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.MoveCardToUnderCard, dca.CardToDestroy.Card, storedResults: yncdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yncdResults))
            {
                dca.SetPostDestroyDestination(this.Card.UnderLocation, cardSource: this.GetCardSource());
            }
        }

        private IEnumerator ReturnCardsToOwnersTrashResponse(GameAction ga)
        {
            IEnumerator coroutine;
            while (this.Card.UnderLocation.HasCards)
            {
                Card c = this.Card.UnderLocation.TopCard;
                MoveCardDestination trashDestination = this.GameController.FindCardController(c).GetTrashDestination();
                coroutine = this.GameController.MoveCard(this.DecisionMaker, c, trashDestination.Location, trashDestination.ToBottom, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}