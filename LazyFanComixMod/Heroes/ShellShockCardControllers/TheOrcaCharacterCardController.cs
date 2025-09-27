using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

namespace LazyFanComix.ShellShock
{
  public class TheOrcaCharacterCardController : PromoDefaultCharacterCardController
  {
    public TheOrcaCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;

      OnPhaseChangeStatusEffect opcse = new OnPhaseChangeStatusEffect(base.Card, "DamageResponse", "At the end of each non-Hero turn, " + this.CharacterCard.AlternateTitleOrTitle + " deals 1 target 2 melee damage.", new TriggerType[]
        {
          TriggerType.DealDamage
        }, this.Card);
      opcse.TurnPhaseCriteria.Phase = Phase.End;
      opcse.TurnTakerCriteria.IsHero = false;
      opcse.UntilStartOfNextTurn(this.TurnTaker);
      opcse.CanEffectStack = true;

      coroutine = this.AddStatusEffect(opcse, true);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    public IEnumerator DamageResponse(PhaseChangeAction p, StatusEffect e)
    {
      return this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => c.Location.IsPlayAreaOf(p.ToPhase.TurnTaker), 1, DamageType.Melee, cardSource: this.GetCardSource());
    }
  }
}