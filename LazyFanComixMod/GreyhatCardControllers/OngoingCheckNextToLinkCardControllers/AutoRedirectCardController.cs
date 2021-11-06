using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class AutoRedirectCardController : GreyhatSharedNetworkCardController
    {
        public AutoRedirectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn("GreyhatRedirectOpportunityPresented", null, null, null);
        }

        public override void AddTriggers()
        {
            this.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => this.CardsLinksAreNextToHeroes.Contains(dd.Target) && dd.Amount > 0 && dd.Amount <= 2 && this.CardsLinksAreNextToNonHero.Count() > 0, "GreyhatRedirectOpportunityPresented", TargetType.SelectTarget, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), optional: true);
        }
    }
}