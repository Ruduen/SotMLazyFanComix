using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.MrFixer
{
    public class MrFixerFlowingStrikeCharacterCardController : PromoDefaultCharacterCardController
    {
        public MrFixerFlowingStrikeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            List<int> powerNumerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1), // Number of targets.
                this.GetPowerNumeral(1, 1), // Amount of damage.
                this.GetPowerNumeral(2, 0), // Amount of damage.
                this.GetPowerNumeral(3, 2) // Amount to draw.
            };

            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            IEnumerator coroutine;

            // Deal target damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[2], DamageType.Melee, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            list.Add(new Function(this.DecisionMaker, "Play an Ongoing or Equipment", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false, cardCriteria: new LinqCardCriteria((Card c) => c.IsOngoing || this.IsEquipment(c), "ongoing or equipment"),
              cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.DecisionMaker) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsOngoing || this.IsEquipment(c)), this.TurnTaker.Name + " cannot draw any cards, so they must play an ongoing or equipment card."));
            list.Add(new Function(this.DecisionMaker, "Draw 2 Cards", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, powerNumerals[3], cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot play any ongoing or equipment cards, so they must draw cards."));
            sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot draw cards or play any ongoing or equipment cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}