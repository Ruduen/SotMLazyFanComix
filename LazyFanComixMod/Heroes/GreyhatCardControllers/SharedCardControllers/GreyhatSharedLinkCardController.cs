using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public abstract class GreyhatSharedLinkCardController : CardController
    {
        protected abstract LinqCardCriteria NextToCriteria { get; }

        public GreyhatSharedLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // For "next to" cards - adjust location.
            this.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false, true);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            // Find what to go next to.
            IEnumerator coroutine = this.SelectCardThisCardWillMoveNextTo(this.NextToCriteria, storedResults, isPutIntoPlay, decisionSources);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = this.UniquePlay();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Post play, do shared "Play a Card" if appropriate.
            // Check if this card was the first played card. Done this way in case the unique part chained.
            // Later instances don't count, for sanity's sake - one-shots shouldn't be destroyed, and the moment you're dealing with a 'second' instances, it's no longer the first.
            if (this.GameController.Game.Journal.PlayCardEntriesThisTurn().FirstOrDefault()?.CardPlayed == this.Card &&
                this.GameController.Game.Journal.PlayCardEntriesThisTurn().Where((PlayCardJournalEntry pcja) => pcja.CardPlayed == this.Card).Count() == 1)
            {
                coroutine = this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected abstract IEnumerator UniquePlay();
    }
}