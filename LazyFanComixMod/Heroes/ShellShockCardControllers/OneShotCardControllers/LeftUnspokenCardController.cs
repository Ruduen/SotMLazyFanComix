using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class LeftUnspokenCardController : CardController
  {
    public LeftUnspokenCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }



    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      List<SelectTurnTakerDecision> sttd = new List<SelectTurnTakerDecision>();
      coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.GainHP(this.CharacterCard, 4, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectHeroToDrawCards(this.DecisionMaker, 1, false, false, storedResults: sttd, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker), cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (this.GetSelectedTurnTaker(sttd) != null)
      {
        coroutine = this.GameController.GainHP(this.GetSelectedTurnTaker(sttd).CharacterCard, 4, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}