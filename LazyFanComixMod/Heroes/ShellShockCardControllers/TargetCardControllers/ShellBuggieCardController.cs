using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class ShellBuggieCardController : VehicleSharedCardController
  {
    public ShellBuggieCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.AddAsPowerContributor();
    }

    public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
    {
      if (cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && cardController.Card.Owner.IsPlayer && !cardController.Card.Owner.IsIncapacitatedOrOutOfGame && !cardController.Card.IsFlipped)
      {
        Power power = new Power(cardController.HeroTurnTakerController, cardController, this.Card.Title + " deals 1 target 3 melee damage.", () => DamageResponse(cardController), 0, null, this.GetCardSource());
        return new Power[]
        {
          power
        };
      }
      return null;
    }

    private IEnumerator DamageResponse(CardController characterCard)
    {
      IEnumerator coroutine;
      int[] powerNums = new int[]
      {
        this.GetPowerNumeral(0, 1),
        this.GetPowerNumeral(0, 3)
      };

      coroutine = this.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), 1, false, 0, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), powerNums[1], DamageType.Melee, powerNums[0], false, powerNums[1], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}