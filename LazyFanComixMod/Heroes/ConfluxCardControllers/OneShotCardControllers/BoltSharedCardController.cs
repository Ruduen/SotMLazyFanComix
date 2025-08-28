using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Conflux
{
  public abstract class BoltSharedCardController : CardController
  {
    public BoltSharedCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    public override IEnumerator Play()
    {
      IEnumerator coroutine;

      coroutine = PlayUnique();
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

      coroutine = PlayShared();
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }

    private IEnumerator PlayShared()
    {
      List<Function> list = new List<Function>();
      SelectFunctionDecision sfd;

      list.Add(new Function(this.HeroTurnTakerController, "Play a card", SelectionType.PlayCard,
          () => this.GameController.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 1, cardSource: this.GetCardSource()), this.HeroTurnTakerController != null && this.CanPlayCardsFromHand(this.HeroTurnTakerController), this.CharacterCard.AlternateTitleOrTitle + " cannot deal damage, so they must play a card."));
      list.Add(new Function(this.HeroTurnTakerController, this.CharacterCard.AlternateTitleOrTitle + " deals 1 target 3 " + UniqueDamageType().ToString() + " damage", SelectionType.DealDamage,
          () => this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, UniqueDamageType(), 1, false, 1, cardSource: this.GetCardSource()),
          null,
          this.TurnTaker.Name + " cannot play a card, so they must deal damage."));
      sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " cannot play a card or deal damage, so" + this.Card.Title + " has no effect.", null, this.GetCardSource());

      return this.GameController.SelectAndPerformFunction(sfd, null, null);
    }

    protected abstract IEnumerator PlayUnique();

    protected abstract DamageType UniqueDamageType();
  }
}