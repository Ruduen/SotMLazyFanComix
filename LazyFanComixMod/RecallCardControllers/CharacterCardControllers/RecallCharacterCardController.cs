using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecallCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public RecallCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            list.Add(new Function(this.DecisionMaker, "Destroy a Card under this card", SelectionType.DestroyCard, () => this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), 1, cardSource: this.GetCardSource()),
                this.Card.UnderLocation.HasCards, this.TurnTaker.Name + " cannot draw any cards, so they must destroy a card under this card."));
            list.Add(new Function(this.DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot destroy any cards this card, so they must draw a card."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw any cards or destroy any cards under this card, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace Incap with something more unique!
    }
}