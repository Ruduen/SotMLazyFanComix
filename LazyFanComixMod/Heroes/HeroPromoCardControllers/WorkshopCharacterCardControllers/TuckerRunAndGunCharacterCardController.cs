using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Tucker
{
    public class TuckerRunAndGunCharacterCardController : PromoDefaultCharacterCardController
    {
        public TuckerRunAndGunCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = new int[] {
                this.GetPowerNumeral(0, 3),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };
            List<DiscardCardAction> dcaResults = new List<DiscardCardAction>();
            IEnumerator coroutine;

            coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNums[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, powerNums[1], false, powerNums[1], dcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            int discardedHandguns = dcaResults.Count((DiscardCardAction dca) => dca.WasCardDiscarded && dca.CardToDiscard.DoKeywordsContain("handgun"));

            if (discardedHandguns > 0)
            {
                for (int i = 0; i < discardedHandguns; i++)
                {
                    coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNums[3], DamageType.Projectile, powerNums[2], false, powerNums[2], cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("No handgun cards were discarded, so no damage will be dealt.", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}