using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class LightningAndThunderCardController : CardController
  {
    public LightningAndThunderCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Lightning, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, (Card c) => !this.IsHeroTarget(c), 1, DamageType.Sonic, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 2, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}