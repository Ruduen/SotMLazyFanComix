using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AbsoluteZero
{
    public class AbsoluteZeroThermalFluxCharacterCardController : PromoDefaultCharacterCardController
    {
        public AbsoluteZeroThermalFluxCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<RevealCardsAction> rcas = new List<RevealCardsAction>();
            IEnumerable<Card> matches = null;
            IEnumerable<Card> nonMatches = null;
            IEnumerator coroutine;

            coroutine = this.GameController.RevealCards(this.DecisionMaker, this.TurnTaker.Deck, (Card c) => c.IsOneShot, 1, rcas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (rcas.FirstOrDefault() != null)
            {
                matches = rcas.FirstOrDefault().MatchingCards;
                nonMatches = rcas.FirstOrDefault().NonMatchingCards;
            }

            if (nonMatches?.Any() == true)
            {
                coroutine = this.GameController.MoveCards(this.DecisionMaker, nonMatches, this.TurnTaker.Trash, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            if (matches?.Any() == true)
            {
                coroutine = this.GameController.PlayCards(this.DecisionMaker, (Card c) => matches.Contains(c), false, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}