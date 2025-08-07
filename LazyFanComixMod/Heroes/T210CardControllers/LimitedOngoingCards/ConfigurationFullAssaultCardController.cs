using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigurationFullAssaultCardController : CardController
  {
    public ConfigurationFullAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayLoadoutResponse, TriggerType.PlayCard);
    }

    private IEnumerator PlayLoadoutResponse(PhaseChangeAction pca)
    {
      return this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("loadout")), cardSource: this.GetCardSource());
    }
  }
}