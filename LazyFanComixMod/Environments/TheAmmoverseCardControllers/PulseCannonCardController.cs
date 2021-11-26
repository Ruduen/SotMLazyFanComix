using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheAmmoverse
{
    public class PulseCannonCardController : CardController
    {
        public PulseCannonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker && !this.Card.Owner.IsHero, EndOfTurnClaimTrigger, TriggerType.MoveCard);
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker && !this.Card.Owner.IsHero, EndOfTurnDamageTrigger, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
        }

        private IEnumerator EndOfTurnClaimTrigger(PhaseChangeAction arg)
        {
            List<SelectTurnTakerDecision> sttd = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine;

            coroutine = this.GameController.SelectHeroTurnTaker(this.DecisionMaker, SelectionType.MoveCardToPlayArea, false, false, storedResults: sttd, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (sttd.Count() > 0 && sttd.First()?.SelectedTurnTaker != null)
            {
                IEnumerable<string> cardPropertyJournalEntryStringList = this.GameController.GetCardPropertyJournalEntryStringList(this.Card, "OverrideTurnTaker", true);
                if (cardPropertyJournalEntryStringList == null || cardPropertyJournalEntryStringList.FirstOrDefault() == null)
                {
                    List<string> list = new List<string>
                    {
                        this.TurnTaker.QualifiedIdentifier,
                        this.Card.QualifiedIdentifier
                    };
                    this.GameController.AddCardPropertyJournalEntry(this.Card, "OverrideTurnTaker", list);
                }
                this.GameController.ChangeCardOwnership(this.Card, sttd.First().SelectedTurnTaker);
            }
        }
        private IEnumerator EndOfTurnDamageTrigger(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            Func<DealDamageAction, int> AmmoNextToThis = (DealDamageAction dda) => this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count() * 3;

            // Trigger to increase damage dealt to self by 2 per elemental. 
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, AmmoNextToThis);

            coroutine = this.DealDamageToHighestHP(null, 1, (Card c) => c.IsHero, (Card c) => 3, DamageType.Sonic, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsVillainCharacterCard, "the villain character card with the highest HP")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3)
            };

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), numerals[1], DamageType.Sonic, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}