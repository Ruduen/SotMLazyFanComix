using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Legacy
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
                  cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.DecisionMaker) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsOneShot), this.TurnTaker.Name + " cannot draw any cards, so they must play a one-shot."));
            list.Add(new Function(this.DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCard(this.HeroTurnTaker, cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any one-shots, so they must draw a card."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw or play any cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}