using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigurationFlashFireCardController : CardController
  {
    public ConfigurationFlashFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn("T210FlashFireOpportunityPresented", null, null, null);
    }

    public override void AddTriggers()
    {
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DidDealDamage && dda.Target == this.CharacterCard && dda.DamageSource?.IsHeroTarget == false, UseLoadoutResponse, TriggerType.UsePower, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay("T210FlashFireOpportunityPresented"), TriggerType.Hidden);
    }

    private IEnumerator UseLoadoutResponse(DealDamageAction dda)
    {
      this.SetCardPropertyToTrueIfRealAction("T210FlashFireOpportunityPresented");
      return this.GameController.SelectAndUsePower(this.HeroTurnTakerController, true, (Power p) => p.CardController.Card.DoKeywordsContain("loadout"), cardSource: this.GetCardSource());
    }
  }
}