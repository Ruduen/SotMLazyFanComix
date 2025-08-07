using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.T210
{
  public class OptimizeLoadoutCardController : OptimizeSharedCardController
  {
    public OptimizeLoadoutCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override LinqCardCriteria AppropriateCards()
    {
      return new LinqCardCriteria((Card c) => c.DoKeywordsContain("loadout"));
    }

    protected override IEnumerator MatchCardAction()
    {
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Projectile, 1, false, 1, cardSource: this.GetCardSource());
    }

  }
}