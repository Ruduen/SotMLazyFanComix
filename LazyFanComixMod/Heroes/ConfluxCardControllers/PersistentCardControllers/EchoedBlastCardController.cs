using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public class EchoedBlastCardController : CardController
  {
    private const string _FirstDamageDealtThisTurn = "FirstDamageDealtThisTurn";

    public EchoedBlastCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
      this.SpecialStringMaker.ShowHasBeenUsedThisTurn(_FirstDamageDealtThisTurn, this.CharacterCard.Title + " has already dealt damage this turn.", this.CharacterCard.Title + " has not yet dealt damage this turn.");
    }

    public override void AddTriggers()
    {
      this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource.Card != null && dda.DamageSource.Card == this.CharacterCard && !this.IsPropertyTrue(_FirstDamageDealtThisTurn), TrackAndDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
      this.AddAfterLeavesPlayAction((GameAction ga) => this.ResetFlagAfterLeavesPlay(_FirstDamageDealtThisTurn), TriggerType.Hidden);
    }

    private IEnumerator TrackAndDamageResponse(DealDamageAction dda)
    {
      IEnumerator coroutine;

      this.SetCardPropertyToTrueIfRealAction(_FirstDamageDealtThisTurn);

      coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, damageType: dda.DamageType, 1, false, 0, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

    }
  }
}