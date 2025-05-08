using Handelabra.Sentinels.Engine;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: TEST!

namespace LazyFanComix.Laggard
{
  public class LaggardCharacterCardController : CharacterCardController
  {
    public string str;

    public LaggardCharacterCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator UsePower(int index = 0)
    {
      List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1)
            };


      // Trigger to increase damage dealt to self by 2 per elemental.
      ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => this.HindsightDamage(powerNumerals[2]));

      // Deal <a> target <b> damage.
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Melee, powerNumerals[0], false, powerNumerals[0], true, cardSource: this.GetCardSource());

      this.RemoveTrigger(tempIncrease);
    }
    private int HindsightDamage(int numeral)
    {
      return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("hindsight")).Count() * numeral;
    }

    public override IEnumerator UseIncapacitatedAbility(int index)
    {
      IEnumerator coroutine;
      switch (index)
      {
        case 0:
          {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location l) => l.IsDeck, storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            Location selectedLocation = this.GetSelectedLocation(storedResults);
            if (selectedLocation != null)
            {
              coroutine = this.GameController.RevealAndReplaceCards(this.HeroTurnTakerController, selectedLocation, 1, new List<Card>(), cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
              break;
            }
            yield break;
          }
        case 1:
          {
            coroutine = this.GameController.SelectHeroToMoveCard(this.HeroTurnTakerController, (HeroTurnTakerController h) => h.HeroTurnTaker.Hand, (HeroTurnTakerController h) => h.TurnTaker.Trash, optionalMoveCard: false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            break;
          }
        case 2:
          {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            coroutine = this.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.PlayTopCard, (Location l) => l.IsDeck && !l.IsEmpty, storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            Location selectedLocation = this.GetSelectedLocation(storedResults);
            if (selectedLocation != null)
            {
              coroutine = this.GameController.PlayTopCardOfLocation(this.FindTurnTakerController(selectedLocation.OwnerTurnTaker), selectedLocation, responsibleTurnTaker: this.HeroTurnTaker, cardSource: this.GetCardSource());
              if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            break;
          }
      }
      yield break;
    }
  }
}