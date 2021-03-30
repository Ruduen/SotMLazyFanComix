using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Legacy
{
    public class LegacyInTheFrayCharacterCardController : PromoDefaultCharacterCardController
    {
        public LegacyInTheFrayCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            list.Add(new Function(this.DecisionMaker, "Play a One-Shot", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false,
                  cardCriteria: new LinqCardCriteria((Card c) => c.IsOneShot, "one-shot"),
                  cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.DecisionMaker) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsOneShot), this.TurnTaker.Name + " cannot discard or draw any cards, so they must play a one-shot."));
            list.Add(new Function(this.DecisionMaker, "Discard a card and Draw a Card", SelectionType.DiscardCard, DiscardAndDraw, this.CanDrawCards(this.DecisionMaker) || this.HeroTurnTaker.HasCardsInHand, this.TurnTaker.Name + " cannot play any one-shots, so they must discard a card and draw a card."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot discard or draw any cards or play any one-shots, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DiscardAndDraw()
        {
            IEnumerator coroutine;
            coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCard(this.HeroTurnTaker, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}