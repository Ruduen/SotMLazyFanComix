using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace RuduenWorkshop.Synthesist
{
    public class SynthesistTurnTakerController : HeroTurnTakerController
    {
        public SynthesistTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {

            IEnumerator coroutine;
            Location relicDeck = this.TurnTaker.FindSubDeck("SynthesistRelicDeck");

            // At the start of game, move all Relics cards into the Relic deck. (This best preserves the 'identity' of the cards.)
            IEnumerable<Card> relicCards = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("relic") && c.Owner == this.TurnTaker);
            coroutine = this.GameController.BulkMoveCards(this, relicCards, relicDeck);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}