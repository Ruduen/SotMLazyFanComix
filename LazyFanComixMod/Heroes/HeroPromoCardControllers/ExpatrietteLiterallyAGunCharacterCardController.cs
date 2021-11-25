using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Expatriette
{
    public class ExpatrietteLiterallyAGunCharacterCardController : PromoDefaultCharacterCardController
    {
        public ExpatrietteLiterallyAGunCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1)
            };
            IEnumerator coroutine;

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[1], DamageType.Projectile),
                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.Card), null, numerals[2], DamageType.Sonic)
            }, null, null, numerals[0], numerals[0], false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCards(this.DecisionMaker, numerals[3], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.GameController.SelectHeroToSelectTargetAndDealDamage(this.DecisionMaker, 2, DamageType.Projectile, false, false, true, false, null, base.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        // No good way to add a new status effect, so add a dummy status effect that lasts until the next turn, nevre triggers, and is only used to check for this effect.
                        OnGainHPStatusEffect giveHeroPowerEffect = new OnGainHPStatusEffect(this.Card, "DontDoThis", "Until the start of your next turn, each Hero gains the power \"This hero deals 1 Target 1 Projectile Damage and 1 Projectile Damage.\"", new TriggerType[] { }, this.TurnTaker, this.Card);
                        giveHeroPowerEffect.UntilStartOfNextTurn(this.TurnTaker);
                        giveHeroPowerEffect.Priority = StatusEffectPriority.Low;
                        giveHeroPowerEffect.SourceCriteria.IsOneOfTheseCards = new List<Card> { this.Card };
                        coroutine = this.AddStatusEffect(giveHeroPowerEffect, true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            // If there are any active status effects which has this card as the source (which should be the only way for this incap to be used), and the target is a hero character, 
            // They get a power! 

            // TODO: If any wacky incap things in the future are done, specifically check for status effects/holders and confirm the status effect type is this simple status effect, and possible a more precise match. But that's not worth it now.
            if (this.GameController.StatusEffectManager.StatusEffectControllers.Where((StatusEffectController sec) => sec.StatusEffect.CardSource == this.Card).Count() > 0 && cardController.Card.IsHeroCharacterCard)
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "This hero deals 1 target 1 projectile damage and 1 projectile damage.", this.PowerResponse(cardController), 0, null, this.GetCardSource()) };
                return list;
            }
            return null;
        }

        private IEnumerator PowerResponse(CardController cardWithPower)
        {
            HeroTurnTakerController hero = cardWithPower.HeroTurnTakerController;
            IEnumerator coroutine;

            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
            };

            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(cardWithPower.GetCardSource(), new DamageSource(this.GameController, cardWithPower.Card), null, numerals[1], DamageType.Projectile),
                new DealDamageAction(cardWithPower.GetCardSource(), new DamageSource(this.GameController, cardWithPower.Card), null, numerals[2], DamageType.Sonic)
            }, null, null, numerals[0], numerals[0], false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DontDoThis()
        {
            yield break;
        }
    }
}