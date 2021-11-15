using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.AbsoluteZero
{
    public class AbsoluteZeroOverchillCharacterCardController : PromoDefaultCharacterCardController
    {
        public AbsoluteZeroOverchillCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>(){
                this.GetPowerNumeral(0, 1), // Amount of damage.
                this.GetPowerNumeral(1, 3) // Cards Drawn.
            };
            IEnumerator coroutine;

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[0], DamageType.Cold, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<Function> list = new List<Function>()
            {
                new Function(this.HeroTurnTakerController, "Play an Equipment", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false,
                  cardCriteria: new LinqCardCriteria((Card c) => this.IsEquipment(c), "equipment"),
                  cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.HeroTurnTakerController) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => this.IsEquipment(c)), this.TurnTaker.Name + " cannot draw any cards or destroy any equipment, so they must play an equipment."),
                new Function(this.HeroTurnTakerController, "Draw 3 Cards", SelectionType.DrawCard, () => DrawAndDestroyCoroutine(powerNumerals[1]),
                this.CanDrawCards(this.HeroTurnTakerController) || this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Owner == this.TurnTaker && this.IsEquipment(c)).Count() > 0,
                this.TurnTaker.Name + " cannot play any equipment, so they must draw cards and destroy an equipment.")
            };

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot draw cards, destroy any equipment, or play any equipment, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DrawAndDestroyCoroutine(int numeral)
        {
            IEnumerator coroutine;
            coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, numeral, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController, cardCriteria: new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && this.IsEquipment(c), "equipment"), false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }


    }
}