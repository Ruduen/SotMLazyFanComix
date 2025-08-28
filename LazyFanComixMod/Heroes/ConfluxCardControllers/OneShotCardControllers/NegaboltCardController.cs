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
      Card target;
      List<DealDamageAction> srd = new List<DealDamageAction>();

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Infernal, 1, false, 1, storedResultsDamage: srd, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if(srd.Where((DealDamageAction dda)=>dda.DidDealDamage && dda.Target.IsInPlayAndHasGameText && dda.Target.HitPoints.Value <= 2).Any())
      {
        target = srd.Where((DealDamageAction dda) => dda.DidDealDamage && dda.Target.IsInPlayAndHasGameText && dda.Target.HitPoints <= 2).Select((DealDamageAction dda) => dda.Target).First();
        coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, target, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    protected override DamageType UniqueDamageType()
    {
      return DamageType.Infernal;
    }
  }
}