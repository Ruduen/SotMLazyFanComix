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
	public class HollowPointCacheCardController : SharedAmmoCardController
	{
		public HollowPointCacheCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator BeforeDamageResponse()
		{
			this.RemoveTrigger(this._usePowerTrigger, false);
			this.AddToTemporaryTriggerList(this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.CardSource.Card == this.GetCardThisCardIsNextTo(true), 2));
			yield return null;
			yield break;
		}

    }
}