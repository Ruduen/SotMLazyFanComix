using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class GlitterGunCardController : SharedHeroGunUnearnedCardController
    {
        public GlitterGunCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ClaimTrigger(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            if (pca.ToPhase.TurnTaker.IsHero)
            {
                List<DestroyCardAction> dcas = new List<DestroyCardAction>();
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());

                coroutine = this.GameController.SelectAndDestroyCards(httc, new LinqCardCriteria((Card c) => !c.IsCharacter && c.Owner == httc.TurnTaker), 1, false, 0, storedResultsAction: dcas, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (dcas.Any((DestroyCardAction dca) => dca.WasCardDestroyed))
                {
                    coroutine = this.ClaimCard(httc);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            };
        }
        protected override TriggerType[] ClaimTriggerTypes()
        {
            return new TriggerType[] { TriggerType.DestroyCard };
        }

    }
}