using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class EndlessPouchesCardController : SharedEquipmentCardController
    {
        public EndlessPouchesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNum = this.GetPowerNumeral(0, 1);
            Card kiwi = this.FindCard("Kiwi");
            IEnumerator coroutine;

            if (kiwi?.IsInPlayAndHasGameText == true)
            {
                coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNum, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else if (kiwi?.Location == this.HeroTurnTaker.Hand || kiwi?.Location == this.TurnTaker.Deck || kiwi?.Location == this.TurnTaker.Trash)
            {
                coroutine = this.GameController.PlayCard(this.DecisionMaker, kiwi, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("Kiwi is not in play, in your deck, or in your trash. Just where did that bird go?", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}