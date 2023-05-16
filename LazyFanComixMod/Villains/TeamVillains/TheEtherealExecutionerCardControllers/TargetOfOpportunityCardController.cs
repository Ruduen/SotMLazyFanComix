using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class TargetOfOpportunityCardController : TheEtherealExecutionerSharedCardController
    {
        public TargetOfOpportunityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> ddaResults = new List<DealDamageAction>();
            IEnumerator coroutine;

            // Trigger to increase damage dealt by 1 per card.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => this.CountObservationCards());

            coroutine = this.DealDamageToLowestHP(this.CharacterCard, 1, (Card c) => (c.IsTarget && !this.IsVillainTarget(c)), (Card c) => 0, DamageType.Projectile, true, storedResults: ddaResults);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            if (ddaResults?.FirstOrDefault()?.DidDestroyTarget == true)
            {
                coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}