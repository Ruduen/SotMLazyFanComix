using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class WreckTheWreckageCardController : CardController
    {
        public WreckTheWreckageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<PlayCardAction> pcaResults = new List<PlayCardAction>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.TurnTaker.Trash && c.IsCover && c.IsTarget, storedResults: pcaResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            PlayCardAction pca = pcaResults.FirstOrDefault((PlayCardAction pca) => pca.WasCardPlayed);
            if (pca?.CardToPlay != null)
            {
                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), pca.CardToPlay, 1, DamageType.Projectile, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("No Cover card was played, so no damage will be dealt.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

    }
}