using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Laggard;
using System.Collections;


namespace LazyFanComix.Laggard
{
  public class EscapeRouteCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard, "Hero"); } }

    public EscapeRouteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddReduceDamageTrigger((Card c) => c == this.GetCardThisCardIsNextTo(true), 2);
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => this.DrawResponse(), new TriggerType[] { TriggerType.DrawCard });
    }

    private IEnumerator DrawResponse()
    {
      IEnumerator coroutine;
      Card c = this.GetCardThisCardIsNextTo();
      if (c?.Owner?.IsHero == true)
      {
        HeroTurnTakerController httc = this.FindHeroTurnTakerController(c.Owner.ToHero());
        if (httc != null)
        {
          coroutine = this.GameController.DrawCards(httc, 2, cardSource: this.GetCardSource());
          if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
      }
      yield break;
    }
  }
}