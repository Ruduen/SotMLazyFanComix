using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;

namespace LazyFanComix.ShellShock
{
  public class TheOrcaCharacterCardController : HeroCharacterCardController
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
      // TODO: Add Tribunal case, since turn taker probably fails during that! 
      opcse.UntilStartOfNextTurn(this.TurnTaker);
      opcse.CanEffectStack = true;

      coroutine = this.AddStatusEffect(opcse, true);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    public IEnumerator DamageResponse(PhaseChangeAction p, StatusEffect e)
    {
      return this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => c.Location.IsPlayAreaOf(p.ToPhase.TurnTaker), 1, DamageType.Melee, cardSource: this.GetCardSource());
    }

    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            coroutine = this.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 1:
          {
            coroutine = this.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 2:
          {
            MakeTargetStatusEffect mtse = new MakeTargetStatusEffect(0, true);
            mtse.CardsToMakeTargets.IsSpecificCard = base.Card;
            mtse.UntilStartOfNextTurn(this.TurnTaker);
            coroutine = this.AddStatusEffect(mtse, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
      }
      yield break;
    }
  }
}