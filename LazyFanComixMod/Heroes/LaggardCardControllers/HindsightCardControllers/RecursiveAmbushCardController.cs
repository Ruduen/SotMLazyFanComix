using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Laggard;
using System.Collections;


namespace LazyFanComix.Laggard
{
  public class RecursiveAmbushCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => !c.IsHero && c.IsTarget, "non-Hero target"); } }

    public RecursiveAmbushCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card == this.GetCardThisCardIsNextTo() && dda.Target.IsHeroCharacterCard && dda.DidDealDamage, (DealDamageAction dda) => HeroPowerResponse(dda.Target), TriggerType.UsePower, TriggerTiming.After);
    }

    private IEnumerator HeroPowerResponse(Card target)
    {
      if (target != null && target.IsHeroCharacterCard == true && target.Owner?.IsHero == true)
      {
        HeroTurnTakerController httc = this.FindHeroTurnTakerController(target.Owner.ToHero());
        IEnumerator coroutine = this.GameController.SelectAndUsePower(httc, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      yield break;
    }
  }
}