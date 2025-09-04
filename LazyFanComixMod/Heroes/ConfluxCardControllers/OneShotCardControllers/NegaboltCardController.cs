using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class NegaboltCardController : BoltSharedCardController
  {
    public NegaboltCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override IEnumerator PlayUnique()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Infernal, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => this.IsOngoing(c), "ongoing"), 1, false, 0, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    protected override DamageType UniqueDamageType()
    {
      return DamageType.Infernal;
    }
  }
}