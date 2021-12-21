using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.BreachMage
{
    public class BreachMageSharedPotentBreachCardController : BreachMageSharedBreachController
    {
        public BreachMageSharedPotentBreachCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            bool criteria(DealDamageAction dd)
            {
                // Increase damage if the spell cast is next to this card.
                // TODO: Also check if the damage is from a Cast effect!
                return (dd.CardSource.Card != null && dd.CardSource.Card.Location == this.Card.NextToLocation && dd.CardSource.Card.IsSpell);
            }
            this.AddTrigger(this.AddIncreaseDamageTrigger(criteria, 1, null, null, false));
        }

        public override IEnumerator UniquePower()
        {
            // Play up to 3 spells.
            return this.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 3, false, new int?(0), new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell")));
        }
    }
}