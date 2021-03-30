using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Greyhat
{
    public class DDoSCardController : GreyhatSharedPlayLinkFirstCardController
    {
        public DDoSCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator PostLinkPlay()
        {
            IEnumerator coroutine;
            List<Card> didDamageCards = new List<Card>();

            // Select players next to links to deal damage.
            SelectCardsDecision selectHeroDecision = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => this.CardsLinksAreNextToHeroes.Contains(c), SelectionType.SelectTargetFriendly, null, false, null, true, true, false, () => NumHeroesToDealDamage(didDamageCards), cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectCardsAndDoAction(selectHeroDecision, (SelectCardDecision scd) => SelectedHeroDealsDamage(scd, didDamageCards), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SelectedHeroDealsDamage(SelectCardDecision scd, List<Card> didDamageCards)
        {
            Card nextTo = scd.SelectedCard;
            if (nextTo != null)
            {
                didDamageCards.Add(nextTo);
                IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, nextTo, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), 1, DamageType.Energy, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private int NumHeroesToDealDamage(List<Card> didDamageCards)
        {
            int num = this.GameController.FindCardsWhere((Card c) => this.CardsLinksAreNextToHeroes.Contains(c)).Except(didDamageCards).Count();
            return didDamageCards.Count() + num;
        }
    }
}