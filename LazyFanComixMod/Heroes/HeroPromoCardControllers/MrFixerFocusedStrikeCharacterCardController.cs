using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.MrFixer
{
    public class MrFixerFocusedStrikeCharacterCardController : PromoDefaultCharacterCardController
    {
        public MrFixerFocusedStrikeCharacterCardController(Card card, TurnTakerController turnTakerController)
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
                this.GetPowerNumeral(2, 1) // Amount to draw.
            };

            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            IEnumerator coroutine;

            // Deal target damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[1], DamageType.Melee, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            list.Add(new Function(this.HeroTurnTakerController, "Play a Style or Tool", SelectionType.PlayCard, () => this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false, cardCriteria: new LinqCardCriteria((Card c) => this.IsOngoing(c) || this.IsEquipment(c), "ongoing or equipment"),
              cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(this.HeroTurnTakerController) && this.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsStyle || c.IsTool), this.TurnTaker.Name + " cannot draw any cards, so they must play a Style or Tool."));
            list.Add(new Function(this.HeroTurnTakerController, "Draw 1 Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, powerNumerals[2], cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot play any Style or Tool, so they must draw a card."));
            sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot draw cards or play any Style or Tool, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}