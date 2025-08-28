using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class SnowboltCardController : BoltSharedCardController
  {
    public SnowboltCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override IEnumerator PlayUnique()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Cold, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    protected override DamageType UniqueDamageType()
    {
      return DamageType.Cold;
    }
  }
}