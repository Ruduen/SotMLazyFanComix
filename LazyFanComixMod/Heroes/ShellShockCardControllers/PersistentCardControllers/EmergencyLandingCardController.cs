using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.ShellShock
{
  public class EmergencyLandingCardController : CardController
  {
    public EmergencyLandingCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card.DoKeywordsContain("vehicle") && dca.CardToDestroy.Card.Owner == this.TurnTaker, OnDestroyResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.GainHP }, TriggerTiming.After);
    }

    private IEnumerator OnDestroyResponse(DestroyCardAction action)
    {
      IEnumerator coroutine;

      coroutine = this.GameController.DrawCards(this.DecisionMaker, 2, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = this.GameController.GainHP(this.CharacterCard, 1, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }


    public override IEnumerator UsePower(int index = 0)
    {
      int[] numerals = new int[]{
          this.GetPowerNumeral(0, 1),
          this.GetPowerNumeral(1, 5)
        };

      List<Card> vehicle = new List<Card>();
      IEnumerator coroutine;

      // Select vehicle target.
      coroutine = this.SelectYourVehicleToDealDamage(vehicle, numerals[1], DamageType.Infernal);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (vehicle.Count > 0)
      {
        DamageSource targetSource = new DamageSource(this.GameController, vehicle.FirstOrDefault());
        coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, targetSource, numerals[1], DamageType.Melee, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        coroutine = this.GameController.DestroyCard(this.DecisionMaker, vehicle.FirstOrDefault(), cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        coroutine = this.GameController.SendMessageAction("No vehicle was selected to deal damage.", Priority.Low, this.GetCardSource(), showCardSource: true);
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }

    public IEnumerator SelectYourVehicleToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType)
    {
      List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
      IEnumerator coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.CardToDealDamage,
          new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && c.IsTarget && c.IsInPlayAndHasGameText && c.DoKeywordsContain("vehicle")),
          storedDecision, false, false,
          new DealDamageAction(this.GetCardSource(), null, null, damageAmount, damageType)
      );
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      if (storedDecision.FirstOrDefault() != null)
      {
        storedResults.Add(storedDecision.FirstOrDefault().SelectedCard);
      }
    }
  }
}