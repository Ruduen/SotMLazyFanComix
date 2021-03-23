using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Parse
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
            int powerNumeral = this.GetPowerNumeral(0, 1);

            List<SelectLocationDecision> storedResultsDeck = new List<SelectLocationDecision>();
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();

            TurnTakerController env = this.FindEnvironment();

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), env.TurnTaker.Trash.NumberOfCards, DamageType.Projectile, powerNumeral, false, 0, storedResultsDamage: storedResultsDamage, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResultsDamage.Count > 0 && storedResultsDamage.Any((DealDamageAction dd) => dd.DidDealDamage) && env.TurnTaker.Trash.NumberOfCards > 0)
            {
                coroutine = this.GameController.ShuffleTrashIntoDeck(env, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}