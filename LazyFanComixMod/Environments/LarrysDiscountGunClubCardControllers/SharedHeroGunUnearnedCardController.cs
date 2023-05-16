using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public abstract class SharedHeroGunUnearnedCardController : CardController
    {
        public SharedHeroGunUnearnedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowListOfCardsNextToCard(this.Card, new LinqCardCriteria((Card c) => c.IsAmmo, "ammo"));
        }

        protected abstract IEnumerator ClaimTrigger(PhaseChangeAction pca);

        protected abstract TriggerType[] ClaimTriggerTypes();

        public override void AddTriggers()
        {
            //// Destroyed trigger should handle destroy cases, which is the only case when the owner
            //this.AddWhenDestroyedTrigger(ResetTrashDestinationToOwner, TriggerType.Hidden);

            //// BeforeLeavesPlayAction should handle all other movement cases, since it seems to be overridden.
            //this.AddBeforeLeavesPlayAction(ResetOwner, TriggerType.Hidden);

            // And every one should have a start-of-hero-turn method for claiming!
            this.AddStartOfTurnTrigger((TurnTaker tt) => !this.Card.Owner.IsHero && tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, ClaimTrigger, ClaimTriggerTypes());
        }

        //private IEnumerator ResetOwner(GameAction ga)
        //{
        //    TurnTaker env = this.FindEnvironment().TurnTaker;
        //    if (this.Card.Owner != env)
        //    {
        //        List<string> list = new List<string>
        //        {
        //            this.TurnTaker.QualifiedIdentifier,
        //            this.Card.QualifiedIdentifier
        //        };
        //        this.GameController.AddCardPropertyJournalEntry(this.Card, "OverrideTurnTaker", list);
        //        this.GameController.ChangeCardOwnership(this.Card, env);
        //    }
        //    yield break;
        //}

        //private IEnumerator ResetTrashDestinationToOwner(DestroyCardAction dca)
        //{
        //    TurnTaker env = this.FindEnvironment().TurnTaker;

        //    if (this.Card.Owner != env)
        //    {
        //        IEnumerator coroutine = ResetOwner(dca);
        //        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        //        if (dca.PostDestroyDestinationCanBeChanged)
        //        {
        //            dca.SetPostDestroyDestination(this.Card.Owner.Trash);
        //        }
        //    }
        //}

        protected IEnumerator ClaimCard(HeroTurnTakerController httc)
        {
            IEnumerator coroutine;

            Card heroWeapon = this.GameController.FindCard(this.Card.Identifier + "Hero");

            coroutine = this.GameController.SwitchCards(this.Card, heroWeapon, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.MoveCard(this.DecisionMaker, heroWeapon, httc.TurnTaker.PlayArea, playCardIfMovingToPlayArea: false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<string> list = new List<string> {
                this.TurnTaker.QualifiedIdentifier,
                heroWeapon.QualifiedIdentifier
            };

            this.GameController.AddCardPropertyJournalEntry(heroWeapon, "OverrideTurnTaker", list);
            this.GameController.ChangeCardOwnership(heroWeapon, httc.TurnTaker);
        }

        public override bool CanOtherCardGoNextToThisCard(Card card)
        {
            return !card.IsAmmo || this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count() < 1;
        }
    }
}