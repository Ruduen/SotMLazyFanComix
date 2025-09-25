using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class RefurbishedBlimpCardController : VehicleSharedCardController
  {
    public RefurbishedBlimpCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.AddAsPowerContributor();
    }

    public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
    {
      if (cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && cardController.Card.Owner.IsPlayer && !cardController.Card.Owner.IsIncapacitatedOrOutOfGame && !cardController.Card.IsFlipped)
      {
        Power power = new Power(cardController.HeroTurnTakerController, cardController, "Draw 1 card. This hero regains 1 HP.", () => Response(cardController), 0, null, this.GetCardSource());
        return new Power[]
        {
          power
        };
      }
      return null;
    }

    private IEnumerator Response(CardController characterCard)
    {

      IEnumerator coroutine;
      HeroTurnTakerController httc;
      int[] powerNums = new int[]
      {
        this.GetPowerNumeral(0, 1)
      };

      if (characterCard?.TurnTaker?.IsPlayer == true)
      {
        // If this is a player, hand is a valid destination.
        httc = this.FindHeroTurnTakerController(characterCard.TurnTaker.ToHero());
        coroutine = this.GameController.DrawCards(httc, 1, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        httc = DecisionMaker;
        coroutine = this.GameController.SendMessageAction(httc.Name + " does not have a hand, so it cannot draw cards.", Priority.Low, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }

      coroutine = this.GameController.GainHP(characterCard.Card, powerNums[0], cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}