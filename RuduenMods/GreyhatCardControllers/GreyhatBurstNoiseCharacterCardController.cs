using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class GreyhatBurstNoiseCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public GreyhatBurstNoiseCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => this.CardsLinksAreNextTo.Contains(c), "links are next to", false, true));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]{
                this.GetPowerNumeral(0,1), // Deal 1 Target
                this.GetPowerNumeral(1,1), // 1 Damage
                this.GetPowerNumeral(2,1), // 1 Other target next to a link
                this.GetPowerNumeral(3,1) // 1 Damage
            };
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();

            IEnumerator coroutine;

            // Deal 1 Target 1 Damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Lightning, powerNumerals[0], false, powerNumerals[0], storedResultsDecisions: scdResults, selectTargetsEvenIfCannotDealDamage: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select other targets next to links to deal damage. 
            if (scdResults.Where((SelectCardDecision scd) => scd.SelectedCard.IsTarget && scd.SelectedCard.IsInPlayAndHasGameText).Count() > 0)
            {
                // Select targets next to links to deal damage.
                SelectCardsDecision selectTargetDecision = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => this.CardsLinksAreNextTo.Contains(c), SelectionType.CardToDealDamage, null, false, powerNumerals[2], true, true, false, () => DoesLinkDamageContinue(scdResults, powerNumerals[2]), cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(selectTargetDecision, (SelectCardDecision scd) => SelectedTargetDealsDamage(scd, scdResults, powerNumerals[3]), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private int DoesLinkDamageContinue(List<SelectCardDecision> targets, int amount)
        {
            if (targets.Where((SelectCardDecision scd) => scd.SelectedCard.IsTarget && scd.SelectedCard.IsInPlayAndHasGameText && !scd.SelectedCard.IsBeingDestroyed).Count() > 0)
            {
                return amount;
            }
            return 0;
        }

        private IEnumerator SelectedTargetDealsDamage(SelectCardDecision source, List<SelectCardDecision> targets, int amount)
        {
            IEnumerator coroutine = this.GameController.DealDamage(this.DecisionMaker, source.SelectedCard, (Card c) => targets.Select((SelectCardDecision scd) => scd.SelectedCard).Contains(c), amount, DamageType.Energy, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        public IEnumerable<Card> CardsLinksAreNextTo
        {
            get
            {
                // Find all links in play. Check for any which are next to a card. Return those owner cards without duplicates.
                return this.GameController.FindCardsWhere((Card c) => c.IsLink && c.IsInPlayAndNotUnderCard && c.Location.IsNextToCard).Select((Card c) => c.Location.OwnerCard).Distinct();
            }
        }

        // TODO: Replace Incap with something more unique!
    }
}