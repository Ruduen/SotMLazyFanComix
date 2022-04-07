using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Parse
{
    public class ParseLaplaceShotCharacterCardController : PromoDefaultCharacterCardController
    {
        public ParseLaplaceShotCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>() {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };

            List<SelectLocationDecision> storedResultsDeck = new List<SelectLocationDecision>();
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();

            // Trigger to increase damage dealt to self by 1 per environment trash card. 
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => (this.FindEnvironment().TurnTaker.Trash.NumberOfCards + this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.IsEnvironment).Count()) * powerNumerals[2]);

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            // Shuffle the environment trash into the environment deck.
            coroutine = this.GameController.ShuffleTrashIntoDeck(this.FindEnvironment(), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }

    }
}