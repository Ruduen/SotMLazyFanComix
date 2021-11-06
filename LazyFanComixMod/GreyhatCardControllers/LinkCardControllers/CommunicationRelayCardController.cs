using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class CommunicationRelayCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == this.TurnTaker, "hero character"); } }

        public CommunicationRelayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }

        protected override IEnumerator UniquePlay()
        {
            // Destroy.
            IEnumerator coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (this.GetCardThisCardIsNextTo() != null && cardController.Card == this.GetCardThisCardIsNextTo())
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "Play 1 Card.", this.PowerResponse(cardController), 0, null, base.GetCardSource(null)) };
                return list;
            }
            return null;
        }

        private IEnumerator PowerResponse(CardController cardWithPower)
        {
            HeroTurnTakerController hero = cardWithPower.HeroTurnTakerController;
            int numeral = this.GetPowerNumeral(0, 1);

            IEnumerator coroutine = this.GameController.SelectAndPlayCardsFromHand(hero, numeral, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}