using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class OverengineeredSlingshotCardController : SharedHeroGunUnearnedCardController
    {
        public OverengineeredSlingshotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ClaimTrigger(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            if (pca.ToPhase.TurnTaker.IsHero)
            {
                List<DealDamageAction> ddas = new List<DealDamageAction>();
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());

                coroutine = this.DealDamage(httc.CharacterCard, httc.CharacterCard, 4, DamageType.Fire, optional: true, storedResults: ddas, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (ddas.Any((DealDamageAction dda) => dda.DidDealDamage && dda.Target == httc.CharacterCard))
                {
                    coroutine = this.ClaimCard(httc);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            };
        }

        protected override TriggerType[] ClaimTriggerTypes()
        {
            return new TriggerType[] { TriggerType.DealDamage };
        }

    }
}