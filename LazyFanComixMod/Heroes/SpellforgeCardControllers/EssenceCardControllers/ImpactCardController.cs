using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Spellforge
{
  public class ImpactCardController : SpellforgeEssenceSharedCardController
  {
    public ImpactCardController(Card card, TurnTakerController turnTakerController)
        : base(card, turnTakerController)
    {
    }

    protected override IEnumerator CoreAction(CardSource cardSource)
    {
      // Deal all 1 melee.
      IEnumerator coroutine = this.DealDamage(this.CharacterCard, (Card c) => c.IsTarget, 1, DamageType.Melee);
      if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
    }
  }
}