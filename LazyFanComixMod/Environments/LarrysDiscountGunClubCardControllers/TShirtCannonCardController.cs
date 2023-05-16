using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class TShirtCannonCardController : SharedHeroGunUnearnedCardController
    {
        public TShirtCannonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator ClaimTrigger(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            if (pca.ToPhase.TurnTaker.IsHero)
            {
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
                List<DiscardCardAction> dcas = new List<DiscardCardAction>();

                coroutine = this.GameController.SelectAndDiscardCards(httc, 1, false, 0, storedResults: dcas, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (dcas.Where((DiscardCardAction dca) => dca.WasCardDiscarded).Count() == 1)
                {
                    coroutine = this.ClaimCard(httc);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }

        protected override TriggerType[] ClaimTriggerTypes()
        {
            return new TriggerType[] { TriggerType.DiscardCard };
        }
    }
}