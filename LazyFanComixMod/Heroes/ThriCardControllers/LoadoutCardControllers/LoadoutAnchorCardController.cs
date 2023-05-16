using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Thri
{
    public class LoadoutAnchorCardController : ThriThirdPowerCardController
    {
        public LoadoutAnchorCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //bool isThirdPower = this.checkThirdPower();
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };

            IEnumerator coroutine;
            DamageSource ds = new DamageSource(this.GameController, this.CharacterCard);
            List<DealDamageAction> ddas = new List<DealDamageAction>()
            {
                new DealDamageAction(this.GetCardSource(), ds, null, powerNumerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), ds, null, powerNumerals[2], DamageType.Toxic)
            };

            if (this.isThirdPower)
            {
                ddas.Add(new DealDamageAction(this.GetCardSource(), ds, null, powerNumerals[3], DamageType.Radiant));
            }

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(ddas, null, null, powerNumerals[0], powerNumerals[0]);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.isThirdPower)
            {
                coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => c != this.Card && (this.IsOngoing(c) || c.IsEnvironment), "ongoing or environment"), 1, false, 0, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}