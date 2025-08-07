using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigurationAutoAttackCardController : CardController
  {
    public ConfigurationAutoAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayLoadoutResponse, TriggerType.PlayCard);
    }

    private IEnumerator PlayLoadoutResponse(PhaseChangeAction pca)
    {
      return this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("loadout")), cardSource: this.GetCardSource());
    }
  }
}