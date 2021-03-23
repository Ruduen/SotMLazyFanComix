using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.AbsoluteZero
{
    public class AbsoluteZeroOverchillCharacterCardController : PromoDefaultCharacterCardController
    {
        public AbsoluteZeroOverchillCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {

            List<Function> list = new List<Function>();
            List<DrawCardAction> dcaResults = new List<DrawCardAction>();
            SelectFunctionDecision sfd;
            List<int> powerNumerals = new List<int>(){
                this.GetPowerNumeral(0, 1), // Amount of damage.
                this.GetPowerNumeral(1, 3) // Cards Drawn.
            };
            IEnumerator coroutine;

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[0], DamageType.Cold, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            list.Add(new Function(this.DecisionMaker, "Play an Equipment", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false,
                  cardCriteria: new LinqCardCriteria((Card c) => this.IsEquipment(c), "equipment"),
                  cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.DecisionMaker) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => this.IsEquipment(c)), this.TurnTaker.Name + " cannot draw any cards, so they must play an equipment."));
            list.Add(new Function(this.DecisionMaker, "Draw 3 Cards", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, powerNumerals[1], storedResults: dcaResults, cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any equipment, so they must draw cards."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw cards or play any equipment, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draws mean destroy one of your equipment cards.
            if (dcaResults.Any((DrawCardAction dca) => dca.DidDrawCard))
            {
                coroutine = this.GameController.SelectAndDestroyCard(this.DecisionMaker, cardCriteria: new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && this.IsEquipment(c), "equipment"), false, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}