using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class LoadoutWhisperCardController : ThriThirdPowerEquipCardController
    {
        public LoadoutWhisperCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //bool isThirdPower = this.checkThirdPower();
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 3)
            };
            DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);

            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, ds, powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // TODO: Third Power message?
            if (this.isThirdPower)
            {
                coroutine = this.GameController.SendMessageAction("This is the third power, so " + this.CharacterCard + " draws " + powerNumerals[2] + " cards.",Priority.Low,this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNumerals[2], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}