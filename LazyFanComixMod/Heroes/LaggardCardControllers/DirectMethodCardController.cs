using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Laggard
{
  public class DirectMethodCardController : CardController
  {
    public DirectMethodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;
      List<DealDamageAction> selfDamageResults = new List<DealDamageAction>();
      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.DealDamageToSelf(this.HeroTurnTakerController, (Card c) => c == this.CharacterCard, 2, DamageType.Psychic, storedResults: selfDamageResults, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      DealDamageAction dda = selfDamageResults.FirstOrDefault();
      if (dda != null && dda.DidDealDamage)
      {
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 5, DamageType.Projectile, 1, false, 1, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

    }
  }
}