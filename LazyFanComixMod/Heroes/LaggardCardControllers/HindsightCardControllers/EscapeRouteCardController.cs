using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;


namespace LazyFanComix.Laggard
{
  public class EscapeRouteCardController : LaggardSharedHindsightCardController
  {
    protected override LinqCardCriteria NextToCriteria
    { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c != this.CharacterCard, "another hero"); } }

    public EscapeRouteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    protected override void AddUniqueTriggers()
    {
      this.AddReduceDamageTrigger((Card c) => c == this.GetCardThisCardIsNextTo(true), 2);
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.HeroTurnTaker, (PhaseChangeAction p) => this.DrawHealResponse(), new TriggerType[] { TriggerType.DrawCard, TriggerType.GainHP });
    }

    private IEnumerator DrawHealResponse()
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
      coroutine = this.GameController.GainHP(c, 3, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      yield break;
    }
  }
}