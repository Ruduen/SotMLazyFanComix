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
      List<DiscardCardAction> dca = new List<DiscardCardAction>();

      coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 5, false, 0, dca, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      
      ITrigger tempIncrease = this.AddToTemporaryTriggerList(this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => dca.Where((DiscardCardAction dca) => dca.IsSuccessful && dca.CardToDiscard.DoKeywordsContain("hindsight")).Count() * 2 + dca.Where((DiscardCardAction dca) => dca.IsSuccessful && !dca.CardToDiscard.DoKeywordsContain("hindsight")).Count()));

      // Deal <a> target <b> damage.
      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), 2, DamageType.Melee, 1, false, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      this.RemoveTemporaryTrigger(tempIncrease);
    }
  }
}