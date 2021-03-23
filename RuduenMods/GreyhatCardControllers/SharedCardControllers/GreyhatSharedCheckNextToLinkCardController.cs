using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public abstract class GreyhatSharedCheckNextToLinkCardController : CardController
    {
        public IEnumerable<Card> CardsLinksAreNextTo
        {
            get
            {
                // Find all links in play. Check for any which are next to a card. Return those owner cards without duplicates.
                return this.GameController.FindCardsWhere((Card c) => c.IsLink && c.IsInPlayAndNotUnderCard && c.Location.IsNextToCard).Select((Card c) => c.Location.OwnerCard).Distinct();
            }
        }

        public IEnumerable<Card> CardsLinksAreNextToHeroes
        {
            get
            {
                // Find all links in play. Check for any next to a card. Check if it is a hero character. Append Greyhat. Make distinct.
                return this.GameController.FindCardsWhere((Card c) => c.IsLink && c.IsInPlayAndNotUnderCard && c.Location.IsNextToCard).Select((Card c) => c.Location.OwnerCard).Where((Card c) => c.IsHeroCharacterCard).Distinct();
            }
        }

        public IEnumerable<Card> CardsLinksAreNextToNonHero
        {
            get
            {
                // Find all links in play. Check for any which are next to a card. Check if it is a non-hero card. Make distinct.
                return this.GameController.FindCardsWhere((Card c) => c.IsLink && c.IsInPlayAndNotUnderCard && c.Location.IsNextToCard).Select((Card c) => c.Location.OwnerCard).Where((Card c) => !c.IsHero).Distinct();
            }
        }

        public GreyhatSharedCheckNextToLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => this.CardsLinksAreNextTo.Contains(c), "links are next to", false, true));
        }
    }
}