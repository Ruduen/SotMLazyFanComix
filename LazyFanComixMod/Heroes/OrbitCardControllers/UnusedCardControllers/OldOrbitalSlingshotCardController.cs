using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class OldOrbitalSlingshotCardController : CardController
    {
        public OldOrbitalSlingshotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowNumberOfCardsUnderCard(this.Card);
        }

        public override void AddTriggers()
        {
            this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.DoKeywordsContain("orbital") && dca.WasCardDestroyed, this.ChangeDestinationResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.ChangePostDestroyDestination }, TriggerTiming.After);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, this.EndOfTurnResponse, new TriggerType[] { });
            this.AddBeforeLeavesPlayActions(ReturnCardsToOwnersTrashResponse);
        }

        private IEnumerator ChangeDestinationResponse(DestroyCardAction dca)
        {
            List<YesNoCardDecision> yncdResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = this.GameController.MakeYesNoCardDecision(this.HeroTurnTakerController, SelectionType.MoveCardToUnderCard, dca.CardToDestroy.Card, storedResults: yncdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yncdResults))
            {
                dca.SetPostDestroyDestination(this.Card.UnderLocation, cardSource: this.GetCardSource());
            }
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> dcaResults = new List<DestroyCardAction>();

            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), storedResults: dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Trigger to increase damage by 2 per destroyed card.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => dcaResults.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Count() * 2);

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Projectile, 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        private IEnumerator ReturnCardsToOwnersTrashResponse(GameAction ga)
        {
            IEnumerator coroutine;
            while (this.Card.UnderLocation.HasCards)
            {
                Card c = this.Card.UnderLocation.TopCard;
                MoveCardDestination trashDestination = this.GameController.FindCardController(c).GetTrashDestination();
                coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, c, trashDestination.Location, trashDestination.ToBottom, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}