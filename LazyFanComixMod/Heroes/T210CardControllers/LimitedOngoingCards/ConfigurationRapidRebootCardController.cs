using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.T210
{
  public class ConfigurationRapidRebootCardController : CardController
  {
    public ConfigurationRapidRebootCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DrawOrRecoverResponse, TriggerType.PlayCard);
    }

    private IEnumerator DrawOrRecoverResponse(PhaseChangeAction pca)
    {
      List<Function> list = new List<Function>();
      SelectFunctionDecision sfd;

      list.Add(new Function(this.HeroTurnTakerController, "Draw 1 Card", SelectionType.DrawCard,
          () => this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource()),
          this.HeroTurnTakerController != null && this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot move any loadout cards from their trash to their hand, so they must draw 1 card."));
      list.Add(new Function(this.HeroTurnTakerController, "Move 1 Loadout Card from your Trash to your Hand", SelectionType.MoveCard,
          () => this.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, this.HeroTurnTaker.Trash, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("loadout"), "loadout"), new List<MoveCardDestination> { new MoveCardDestination(this.HeroTurnTaker.Hand) }, cardSource: this.GetCardSource()),
          this.TurnTakerController != null && this.TurnTaker.Trash.Cards.Where((Card c) => c.DoKeywordsContain("loadout")).Any(),
          this.TurnTaker.Name + " cannot draw any cards, so they must move a Loadout card from their trash to their hand."));
      sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot draw any cards or move any loadout cards from their trash to their hand, so" + this.Card.Title + " has no effect.", null, this.GetCardSource());

      return this.GameController.SelectAndPerformFunction(sfd, null, null);
    }
  }
}