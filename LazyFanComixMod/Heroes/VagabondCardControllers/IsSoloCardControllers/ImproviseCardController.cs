using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Vagabond
{
    public class ImproviseCardController : SharedSoloCardController
    {
        public ImproviseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.DrawCards(this.DecisionMaker, 2, true, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            return this.RevealCards_SelectSome_MoveThem_ReturnTheRest(this.DecisionMaker, this.DecisionMaker, this.TurnTaker.Deck, (Card c) => c.IsOneShot, 1, 1, true, true, true, "one-shot");
            //return this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, this.TurnTaker.Deck, false, true, true, new LinqCardCriteria((Card c) => c.IsOneShot, "one-shot"), 1);
        }

        protected override IEnumerator OnPlayIfNotSolo()
        {
            return this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero && tt != this.TurnTaker), SelectionType.DrawCard, (TurnTaker tt) => this.GameController.DrawCard(tt.ToHero(), true, cardSource: this.GetCardSource()), 2, false, 0, cardSource: this.GetCardSource());
        }
    }
}