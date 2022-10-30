using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace LazyFanComix.Vagabond
{
    public class VagabondElusiveCharacterCardController : PromoDefaultCharacterCardController
    {

        public VagabondElusiveCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        private IEnumerable<HeroTurnTakerController> SoloTurnTakerControllers()
        {
            return this.GameController.HeroTurnTakerControllers.Where((HeroTurnTakerController ttc) => this.GameController.CanActivateEffect(ttc, "vagabond solo"));
        }

        public override void AddTriggers()
        {
            this.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && md.CardSource.Card.Owner.IsHero, new System.Func<MakeDecisionsAction, IEnumerator>(this.RemoveDecisionsFromMakeDecisionsResponse), TriggerType.RemoveDecision, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Until the end of your turn, solo. Draw or play. 
            IEnumerator coroutine;

            if (this.TurnTaker.IsHero)
            {
                ActivateEffectStatusEffectWithTextOverride soloEffect = new ActivateEffectStatusEffectWithTextOverride(this.TurnTaker, this.Card, "vagabond solo");
                soloEffect.UntilThisTurnIsOver(this.Game);

                coroutine = this.AddStatusEffect(soloEffect);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("No, this can't lock off the environment. This effect should technically say \"in a hero play area\". Why didn't I write it that way? Look, I'm not the one on trial here.", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.DrawACardOrPlayACard(this.DecisionMaker, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            foreach (HeroTurnTakerController httc in SoloTurnTakerControllers())
            {
                md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner != httc.TurnTaker && d.HeroTurnTakerController.TurnTaker == httc.TurnTaker);
                md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner == httc.TurnTaker && d.HeroTurnTakerController.TurnTaker != httc.TurnTaker);
            }
            yield return this.DoNothing();
            yield break;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            // NOT WORTH IT: Needs specific override case if we ever want Handelabra support.
            return this.AskIfTurnTakerIsVisibleToCardSource(card.Owner, cardSource);
        }

        /// <summary>
        /// Working before really crazy adjustments for environment implementation.
        /// </summary>
        public override bool? AskIfTurnTakerIsVisibleToCardSource(TurnTaker tt, CardSource cardSource)
        {

            if (cardSource == null || !cardSource.Card.IsHero || !tt.IsHero)
            {
                return true;
            }

            IEnumerable<TurnTaker> soloTurnTakerControllers = SoloTurnTakerControllers().Select((HeroTurnTakerController ttc) => ttc.TurnTaker);

            if (soloTurnTakerControllers.Contains(cardSource.Card.Owner))
            {
                // If the owner of the effect is solo, make sure it matches.
                return tt == cardSource.Card.Owner;
            }
            // If the owner of the effect is not solo, make sure it's someone not isolated.
            return !soloTurnTakerControllers.Contains(tt);
        }

        // NOT WORTH IT: Environment hero case. 
        //public override bool? AskIfTurnTakerIsVisibleToCardSource(TurnTaker tt, CardSource cardSource)
        //{

        //    if (cardSource == null)
        //    {
        //        return true;
        //    }

        //    IEnumerable<TurnTaker> soloTurnTakerControllers = SoloTurnTakerControllers().Select((TurnTakerController ttc) => ttc.TurnTaker);

        //    //if (soloTurnTakerControllers.Contains(tt))
        //    //if (soloTurnTakerControllers.Contains(cardSource.Card.Owner))

        //            if (soloTurnTakerControllers.Contains(cardSource.Card.Owner))
        //            {
        //                If the owner of the effect is solo, make sure the target either is the same or is not a hero.
        //        return cardSource.Card.Owner == tt || !tt.IsHero;
        //            }
        //            else if (cardSource.Card.Owner.IsHero)
        //            {
        //                If the owner of the effect is not solo and is a hero, make sure it's someone not isolated or is not a hero.
        //                return !soloTurnTakerControllers.Contains(tt);
        //            }

        //    return true;
        //}

        private class ActivateEffectStatusEffectWithTextOverride : ActivateEffectStatusEffect
        {
            public ActivateEffectStatusEffectWithTextOverride(TurnTaker turnTaker, Card card, string effectName) : base(turnTaker, card, effectName)
            {
            }

            // Commented out since it breaks the 'used by multiple characters' logic.

            //public override bool IsRedundant(IEnumerable<StatusEffect> otherStatusEffects)
            //{
            //    // Get all preliminary matches. 
            //    IEnumerable<ActivateEffectStatusEffectWithTextOverride> otherRelevantOverrides = otherStatusEffects.Where((StatusEffect se) => this.IsSameAs(se) && se is ActivateEffectStatusEffectWithTextOverride).Select((StatusEffect se) => (ActivateEffectStatusEffectWithTextOverride)se);

            //    // Also confirm turn taker.
            //    bool test = otherRelevantOverrides.Any((ActivateEffectStatusEffectWithTextOverride aesewto) => TurnTaker == aesewto.TurnTaker);
            //    return test;
            //}

            //public override bool CombineWithExistingInstance
            //{
            //    get { return true; }
            //}

            public override string ToString()
            {
                string name = "";
                if (this.TurnTaker != null)
                {
                    name = this.TurnTaker.Name;
                }
                else if (this.Card != null)
                {
                    name = this.Card.AlternateTitleOrTitle;
                }
                return string.Format("{0} cannot affect or be affected other Heroes this turn.", name);
            }
        }
        // TODO: Replace Incap with something more unique!
    }
}