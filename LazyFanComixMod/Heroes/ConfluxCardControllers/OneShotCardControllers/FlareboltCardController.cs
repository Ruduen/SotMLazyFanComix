using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class FlareboltCardController : BoltSharedCardController
  {
    public FlareboltCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override IEnumerator PlayUnique()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Fire, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    protected override DamageType UniqueDamageType()
    {
      return DamageType.Fire;
    }
  }
}