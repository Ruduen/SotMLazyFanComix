using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public abstract class SharedSoloCardController : CardController
    {
        public SharedSoloCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        private bool IsSolo()
        {
            List<Card> otherActiveHeroes = this.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.IsActive && c != this.CharacterCard && this.GameController.IsCardVisibleToCardSource(c, this.GetCardSource())).ToList();
            if (!otherActiveHeroes.Any())
            {
                return true;
            }
            return false;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = OnPlayAlways();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (IsSolo())
            {
                coroutine = OnPlayIfSolo();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.CharacterCard.AlternateTitleOrTitle + " is not the only active hero, so " + IfNotSoloMessage() + ".",Priority.Low,this.GetCardSource(),showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }



        protected abstract string IfNotSoloMessage();

        protected abstract IEnumerator OnPlayAlways();
        protected abstract IEnumerator OnPlayIfSolo();

    }
}