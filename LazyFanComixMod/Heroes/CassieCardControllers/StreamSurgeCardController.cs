﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Cassie
{
    public class StreamSurgeCardController : CassieRiverSharedCardController
    {
        public StreamSurgeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            // Up to 3 targets 1 damage.

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Projectile, new int?(3), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // You may play a card.
            coroutine = this.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, true, new int?(0), null, false, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}