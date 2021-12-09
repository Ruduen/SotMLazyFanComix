using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class OverengineeredSlingshotCardController : SharedHeroGunCardController
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
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
                List<Function> list = new List<Function>()
                {
                    new Function(httc, httc.CharacterCard.Title + " deals themselves 4 Fire Damage to Claim " + this.Card.Title, SelectionType.DealDamage, () => DamageAndClaim(httc), httc.NumberOfCardsInHand >= 3)
                };

                SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, httc, list, true, null, httc.Name + " cannot discard 3 cards, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

                coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            };
        }
        private IEnumerator DamageAndClaim(HeroTurnTakerController httc)
        {
            List<DealDamageAction> ddas = new List<DealDamageAction>();
            IEnumerator coroutine;

            coroutine = this.GameController.DealDamageToSelf(httc, (Card c) => c.Owner == httc.TurnTaker && c.IsHeroCharacterCard, 4, DamageType.Fire, storedResults: ddas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (ddas.Any((DealDamageAction dda)=>dda.DidDealDamage))
            {
                coroutine = this.ClaimCard(httc);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected override TriggerType[] ClaimTriggerTypes()
        {
            return new TriggerType[] { TriggerType.DealDamage };
        }

    }
}