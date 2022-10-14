using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class ImbuedDetonationCardController : SharedSoloCardController
    {
        public ImbuedDetonationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override IEnumerator OnPlayAlways()
        {
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Energy, 1, false, 1, cardSource: this.GetCardSource());
        }

        protected override IEnumerator OnPlayIfSolo()
        {
            List<DiscardCardAction> dcaResults = new List<DiscardCardAction>();
            IEnumerator coroutine;

            coroutine = this.SelectAndDiscardCards(this.DecisionMaker, 2, true, null, dcaResults);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if(dcaResults.Where((DiscardCardAction dca)=>dca.WasCardDiscarded).Count() == 2)
            {
                coroutine = this.Play();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected override string IfNotSoloMessage()
        {
            return "no cards can be discarded to repeat this card's text";
        }
    }
}