using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Laggard
{
  public class RetrophetCardController : CardController
  {

    public RetrophetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddTrigger<MoveCardAction>(CheckHero, DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);
      this.AddTrigger<MoveCardAction>(CheckNonHero, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
    }

    private bool CheckHero(MoveCardAction pca)
    {
      return pca.ResponsibleTurnTaker == this.HeroTurnTaker &&
        pca.CardToMove.DoKeywordsContain("hindsight") &&
        pca.IsSuccessful &&
        pca.CardToMove.Location.IsNextToCard &&
        pca.CardToMove.Location.OwnerCard.IsHeroCharacterCard &&
        pca.CardToMove.Location.OwnerTurnTaker.IsHero;
    }

    private bool CheckNonHero(MoveCardAction pca)
    {
      return pca.ResponsibleTurnTaker == this.HeroTurnTaker &&
        pca.CardToMove.DoKeywordsContain("hindsight") &&
        pca.IsSuccessful &&
        pca.CardToMove.Location.IsNextToCard &&
        !pca.CardToMove.Location.OwnerCard.IsHero &&
        pca.CardToMove.Location.OwnerCard.IsTarget;
    }

    private IEnumerator DrawCardResponse(MoveCardAction pca)
    {
      return this.GameController.DrawCard(pca.CardToMove.Location.OwnerTurnTaker.ToHero(), false, cardSource: this.GetCardSource());
    }
    private IEnumerator DealDamageResponse(MoveCardAction pca)
    {
      return this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), pca.CardToMove.Location.OwnerCard, 2, DamageType.Projectile, cardSource: this.GetCardSource());
    }

    public override IEnumerator UsePower(int index = 0)
    {
      List<MoveCardAction> mcaResult = new List<MoveCardAction>();
      int[] powerNumerals =
        {
          this.GetPowerNumeral(0, 3),
        };
      IEnumerator coroutine;
      coroutine = this.GameController.DiscardTopCards(this.HeroTurnTakerController, this.HeroTurnTaker.Deck, powerNumerals[0], mcaResult, cardSource: this.GetCardSource());
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      Card[] hindsights = mcaResult?.Where((MoveCardAction mca) => mca.CardToMove.DoKeywordsContain("hindsight") && mca.WasCardMoved && mca.CardToMove.Location.IsTrash).Select((MoveCardAction mca) => mca.CardToMove).ToArray();
      if (hindsights.Count() > 0)
      {
        coroutine = this.GameController.SelectAndMoveCard(this.HeroTurnTakerController, ((Card c) => hindsights.Contains(c)), this.HeroTurnTaker.PlayArea, false, true, false, true, cardSource: this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
      else
      {
        coroutine = this.GameController.SendMessageAction("No Hindsight cards were discarded, so no hindsight card can be played.", Priority.Low, this.GetCardSource());
        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
      }
    }


  }
}