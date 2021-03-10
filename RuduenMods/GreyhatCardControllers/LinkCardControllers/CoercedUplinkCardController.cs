using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class CoercedUplinkCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsTarget && !c.IsHero, "non-hero target"); } }

        public CoercedUplinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            Card nextTo = this.GetCardThisCardIsNextTo(true);
            IEnumerator coroutine;
            if (nextTo != null)
            {
                // Damage.
                coroutine = this.DealDamage(this.CharacterCard, nextTo, 2, DamageType.Lightning, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
    }
}