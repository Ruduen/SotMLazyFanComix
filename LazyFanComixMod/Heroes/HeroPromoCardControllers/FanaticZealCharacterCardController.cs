using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Fanatic
{
    public class FanaticZealCharacterCardController : PromoDefaultCharacterCardController
    {
        public FanaticZealCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            List<int> powerNumerals = new List<int>()
            {
                this.GetPowerNumeral(0, 2), // Number of targets.
                this.GetPowerNumeral(1, 1), // Amount of damage.
            };

            IEnumerator coroutine;

            // Deal target damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Radiant, powerNumerals[0], false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[1], DamageType.Radiant, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            list.Add(new Function(this.HeroTurnTakerController, "Use a Power", SelectionType.UsePower, () => this.GameController.SelectAndUsePower(this.HeroTurnTakerController, false, cardSource: this.GetCardSource()), this.GameController.CanUsePowers(this.HeroTurnTakerController, cardSource: this.GetCardSource()), this.TurnTaker.Name + " cannot draw any cards, so they must use a power."));
            list.Add(new Function(this.HeroTurnTakerController, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot use any powers, so they must draw a card."));
            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " use any powers or draw any cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}