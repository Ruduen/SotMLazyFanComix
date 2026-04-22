using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.T210
{
  public class OptimizeWeaponryCardController : OptimizeSharedCardController
  {
    public OptimizeWeaponryCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override LinqCardCriteria AppropriateCards()
    {
      return new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && this.GameController.DoesCardContainKeyword(c, "loadout"));
    }

    protected override IEnumerator MatchCardAction()
    {
      return this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 3, DamageType.Projectile, 1, false, 1, cardSource: this.GetCardSource());
    }

  }
}