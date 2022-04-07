using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class OrbitalSlingshotCardController : CardController
    {
        public OrbitalSlingshotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            Func<int> increaseAmount = () => { return this.GameController.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsInPlay && (c.IsCover || c.DoKeywordsContain("orbital"))).Count() * 2; };

            // Trigger to increase damage by 1 per cover in trash. 
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => increaseAmount());

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Sonic, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            coroutine = this.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => c == this.CharacterCard, 2, DamageType.Sonic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}