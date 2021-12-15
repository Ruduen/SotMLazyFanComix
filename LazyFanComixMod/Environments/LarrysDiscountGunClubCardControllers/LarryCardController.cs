using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class LarryCardController : CardController
    {
        public LarryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnHealDamageTrigger, new TriggerType[] { TriggerType.GainHP, TriggerType.DealDamage });
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndOfTurnPlayTrigger, new TriggerType[] { TriggerType.PlayCard });
            this.AddBeforeDestroyAction(DestroyedPlayVillainTrigger);
        }


        private IEnumerator StartOfTurnHealDamageTrigger(PhaseChangeAction pca)
        {
            IEnumerator coroutine;


            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => c == this.Card, this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsGun).Count(), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.Card, (Card c) => c != this.Card && !c.IsDevice, 1, DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator EndOfTurnPlayTrigger(PhaseChangeAction pca)
        {
            IEnumerator coroutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, showMessage: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DestroyedPlayVillainTrigger(GameAction ga)
        {
            IEnumerator coroutine;
            if (this.Card.HitPoints >= 1)
            {
                coroutine = this.PlayTheTopCardOfTheVillainDeckResponse(ga);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}