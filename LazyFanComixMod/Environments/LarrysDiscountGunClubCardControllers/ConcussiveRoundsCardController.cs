using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class ConcussiveRoundsCardController : SharedAmmoCardController
    {
        public ConcussiveRoundsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator BeforeDamageResponse()
        {
            this.RemoveTrigger(this._usePowerTrigger, false);
            this.AddToTemporaryTriggerList(this.AddTrigger<DealDamageAction>((DealDamageAction dda) => this.IsThisCardNextToCard(dda.CardSource.Card) && dda.DidDealDamage, SelfDamageResponse, TriggerType.DealDamage, TriggerTiming.After));
            yield return null;
            yield break;
        }

        protected IEnumerator SelfDamageResponse(DealDamageAction dd)
        {
            IEnumerator coroutine;

            // Self Damage Response
            coroutine = this.DealDamage(dd.Target, dd.Target, 2, DamageType.Sonic, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}