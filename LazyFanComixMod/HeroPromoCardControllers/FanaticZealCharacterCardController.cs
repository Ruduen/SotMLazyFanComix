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
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Radiant, powerNumerals[0], false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, powerNumerals[1], DamageType.Radiant, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            list.Add(new Function(this.DecisionMaker, "Use a Power", SelectionType.UsePower, () => this.GameController.SelectAndUsePower(this.DecisionMaker, false, cardSource: this.GetCardSource()), this.GameController.CanUsePowers(this.DecisionMaker, cardSource: this.GetCardSource()), this.TurnTaker.Name + " cannot draw any cards, so they must use a power."));
            list.Add(new Function(this.DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource()), this.CanDrawCards(this.DecisionMaker), this.TurnTaker.Name + " cannot use any powers, so they must draw a card."));
            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " cannot discard or draw any cards or play any one-shots, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}