using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
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
                List<YesNoCardDecision> yncds = new List<YesNoCardDecision>();
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());

                coroutine = this.GameController.MakeYesNoCardDecision(httc, SelectionType.DealDamageSelf, this.Card, storedResults: yncds, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (yncds.Any((yncd) => yncd.Answer == true))
                {
                    coroutine = this.GameController.DealDamage(httc, httc.CharacterCard, (Card c) => c == httc.CharacterCard, 3, DamageType.Fire, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

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