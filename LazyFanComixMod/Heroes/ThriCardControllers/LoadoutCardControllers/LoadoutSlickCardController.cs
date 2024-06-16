using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class LoadoutSlickCardController : ThriThirdPowerEquipCardController
    {
        public LoadoutSlickCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //bool isThirdPower = this.checkThirdPower();
            List<DealDamageAction> ddas = new List<DealDamageAction>();
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3),
                this.GetPowerNumeral(2, 2),
                this.GetPowerNumeral(3, 3)
            };
            DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);

            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, ds, powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[0], storedResultsDamage: ddas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // TODO: Third Power message?
            if (this.isThirdPower)
            {
                coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.CharacterCard + " deals " + powerNumerals[2] + " additional targets damage.", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                IEnumerable<Card> damagedTargets = ddas.Select((DealDamageAction dda) => dda.Target);
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, ds, powerNumerals[3], DamageType.Projectile, powerNumerals[2], false, 0, additionalCriteria: (Card c) => !damagedTargets.Contains(c), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}