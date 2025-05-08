using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Laggard
{
  public class ForegoneConclusionCardController : CardController
  {
    public ForegoneConclusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.HeroUsingPower == this.HeroTurnTakerController, (UsePowerAction upa) => this.GameController.DrawCard(this.HeroTurnTaker, true, cardSource: this.GetCardSource()), TriggerType.DrawCard, TriggerTiming.After);
    }

    public override IEnumerator UsePower(int index = 0)
    {
      IEnumerator coroutine;
      List<SelectLocationDecision> sld = new List<SelectLocationDecision>();
      coroutine = this.FindVillainDeckEx(this.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, sld, (Location l) => true);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      Location selectedLocation = this.GetSelectedLocation(sld);
      if (selectedLocation != null)
      {
        coroutine = this.GameController.RevealAndReplaceCards(TurnTakerController, selectedLocation, 1, new List<Card>(), cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.PlayTopCardOfLocation(this.FindTurnTakerController(selectedLocation.OwnerTurnTaker), selectedLocation, true, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }
  }
}