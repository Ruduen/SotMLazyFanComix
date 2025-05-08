using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Laggard
{
  public class SlowChaseSequenceCardController : CardController
  {
    public SlowChaseSequenceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      List<DealDamageAction> damageResults = new List<DealDamageAction>();
      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Melee, 1, false, 1, storedResultsDamage: damageResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      DealDamageAction dda = damageResults.FirstOrDefault();
      if (dda != null && dda.DidDealDamage && dda.Target != null && !dda.Target.IsCharacter && !this.GameController.IsCardIndestructible(dda.Target) && dda.Target.IsInPlayAndHasGameText)
      {
        coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, dda.Target, dda.Target.NativeDeck, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }
  }
}