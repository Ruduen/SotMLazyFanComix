using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public abstract class GreyhatSharedNetworkCardController : GreyhatSharedCheckNextToLinkCardController
    {
        public GreyhatSharedNetworkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Do the unique thing. 
            coroutine = UniquePlay();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Post play, do shared "Draw 2 Cards" if appropriate.
            // Check if this card was the first played card. Done this way in case the unique part chained. 
            // Later instances don't count, for sanity's sake - one-shots shouldn't be destroyed, and the moment you're dealing with a 'second' instances, it's no longer the first. 
            if (this.GameController.Game.Journal.PlayCardEntriesThisTurn().FirstOrDefault()?.CardPlayed == this.Card && 
                this.GameController.Game.Journal.PlayCardEntriesThisTurn().Where((PlayCardJournalEntry pcja) => pcja.CardPlayed == this.Card).Count() == 1)
            {
                coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 2, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        protected virtual IEnumerator UniquePlay()
        {
            yield break;
        }
    }
}