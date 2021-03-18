using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class AutoRedirectHubCardController : GreyhatSharedLinkCheckNextToLinkCardController
    {
        public AutoRedirectHubCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowHasBeenUsedThisTurn("GreyhatRedirectOpportunityPresented", null, null, null);
        }

        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c != null && c.IsHero && c.IsCharacter && c.Owner == this.TurnTaker, "your hero"); } }
        public override IEnumerator Play()
        {
            // Destroy.
            IEnumerator coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        protected override void AddUniqueTriggers()
        {
            this.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => this.CardsLinksAreNextToHeroes.Contains(dd.Target) && dd.Amount > 0 && dd.Amount <= 2 && this.CardsLinksAreNextToNonHero.Count() > 0, "GreyhatRedirectOpportunityPresented", TargetType.SelectTarget, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), optional: true);
        }
    }
}