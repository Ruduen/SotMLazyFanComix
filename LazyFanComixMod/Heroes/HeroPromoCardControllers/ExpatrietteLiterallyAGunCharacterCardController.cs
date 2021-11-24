using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteLiterallyAGunCharacterCardController : PromoDefaultCharacterCardController
    {
        public ExpatrietteLiterallyAGunCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };
            List<DealDamageAction> ddas = new List<DealDamageAction>();
            IEnumerator coroutine;

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[2], DamageType.Projectile)
            }, null, null, numerals[0], numerals[0], false, ddas);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), numerals[3], DamageType.Projectile, 1, false, 0, additionalCriteria: (Card c) => !ddas.Select((DealDamageAction dda) => dda.OriginalTarget).Contains(c), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}