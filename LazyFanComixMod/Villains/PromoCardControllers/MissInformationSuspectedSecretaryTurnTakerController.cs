using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.MissInformation
{
    public class MissInformationSuspectSecretaryTurnTakerController : TurnTakerController
    {
        public MissInformationSuspectSecretaryTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }
        public override IEnumerator StartGame()
        {
            IEnumerator coroutine;

            coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " plays 2 targets!", Priority.Low, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.IsTarget), 2, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}