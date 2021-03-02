using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public abstract class SoulbinderSharedMultiCharacterCardController : HeroCharacterCardController
    {
        public SoulbinderSharedMultiCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        // Apparently not necessary since it's now Gracefully handled by the core logic?
        //public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        //{
        //	if (!this.CardWithoutReplacements.IsFlipped)
        //	{
        //		IEnumerator coroutine = this.DoAction(new RemoveTargetAction(base.GameController, base.CardWithoutReplacements, true));

        //		if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        //		this.RemoveAllTriggers(true, true, true, false, false);
        //	}
        //	yield break;
        //}

        // TODO: Replace Incap with something more unique!
    }
}