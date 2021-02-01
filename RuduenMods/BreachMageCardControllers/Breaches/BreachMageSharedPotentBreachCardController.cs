using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.BreachMage
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
                return (dd.CardSource.Card.Location == this.Card.NextToLocation && dd.CardSource.Card.IsSpell);
            }
            this.AddTrigger(this.AddIncreaseDamageTrigger(criteria, 1, null, null, false));
        }

        public override IEnumerator UseOpenPower()
        {
            // Play a card
            IEnumerator coroutine;
            coroutine = this.SelectAndPlayCardFromHand(this.DecisionMaker);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw a card.
            coroutine = this.DrawCards(this.DecisionMaker, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}