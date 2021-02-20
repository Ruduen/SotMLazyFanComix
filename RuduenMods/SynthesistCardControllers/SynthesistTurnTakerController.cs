using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            // At the start of game, flip the relevant cards, then move all Relics cards into the Relic deck. (This best preserves the 'identity' of the cards.)
            IEnumerable<Card> relicCards = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("relic") && c.Owner == this.TurnTaker);
            IEnumerable<CardController> relicCardControllers = this.GameController.FindCardControllersWhere((Card c) => relicCards.Contains(c));

            coroutine = this.GameController.FlipCards(relicCardControllers);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.MoveCards(this, relicCards, this.HeroTurnTaker.PlayArea);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }
    }
}