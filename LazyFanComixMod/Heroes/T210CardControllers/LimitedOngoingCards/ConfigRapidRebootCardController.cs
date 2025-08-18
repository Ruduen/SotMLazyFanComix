using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace LazyFanComix.T210
{
  public class ConfigRapidRebootCardController : CardController
  {
    public ConfigRapidRebootCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
    }

    public override void AddTriggers()
    {
      this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DrawOrRecoverResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.GainHP });
    }

    private IEnumerator DrawOrRecoverResponse(PhaseChangeAction pca)
    {
      List<Function> list = new List<Function>();
      SelectFunctionDecision sfd;

      list.Add(new Function(this.HeroTurnTakerController, this.CharacterCard.Title + "Gains 1 HP", SelectionType.GainHP,
          () => this.GameController.GainHP(this.CharacterCard, 1, cardSource: this.GetCardSource()), null, this.TurnTaker.Name + " cannot move any loadout cards to their hand, so they must regain HP."));
      list.Add(new Function(this.HeroTurnTakerController, "Move 1 Loadout Card from your Trash to your Hand", SelectionType.MoveCard,
          () => this.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, this.HeroTurnTaker.Trash, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("loadout"), "loadout"), new List<MoveCardDestination> { new MoveCardDestination(this.HeroTurnTaker.Hand) }, cardSource: this.GetCardSource()),
          this.TurnTakerController != null && this.TurnTaker.Trash.Cards.Where((Card c) => c.DoKeywordsContain("loadout")).Any(),
          this.TurnTaker.Name + " cannot regain HP, so they must move a Loadout card from their trash to their hand."));
      sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot regain HP or move any loadout cards from their trash to their hand, so" + this.Card.Title + " has no effect.", null, this.GetCardSource());

      return this.GameController.SelectAndPerformFunction(sfd, null, null);
    }
  }
}