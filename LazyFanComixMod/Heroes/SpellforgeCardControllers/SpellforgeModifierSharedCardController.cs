using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Spellforge
{
    public abstract class SpellforgeModifierSharedCardController : CardController
    {
        public SpellforgeModifierSharedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected CardController _cardControllerActivatingModifiers;
        protected Card _cardToModify;

        protected bool CoreDealDamageActionCriteria(DealDamageAction dda)
        {
            // Damage is being dealt, card source is non-null, 
            return dda.CanDealDamage && dda.CardSource != null &&
                // either card being modified matches or power being modified exists and matches.
                (dda.CardSource.Card == this._cardToModify);
        }
        public ITrigger AddModifierTrigger(CardController cardControllerActivatingModifiers, Card cardToModify)
        {
            _cardControllerActivatingModifiers = cardControllerActivatingModifiers;
            _cardToModify = cardToModify;
            return AddModifierTriggerOverride();
        }

        public void RemoveModifierTrigger()
        {
            _cardControllerActivatingModifiers = null;
            _cardToModify = null;
            ClearOtherValues();
            return;
        }

        public ITrigger AddDesignatePlayTrigger(CardController cardControllerActivatingModifiers)
        {
            Func<PlayCardAction, IEnumerator> updateCardToModify = (pca) =>
            {
                _cardToModify = pca.CardToPlay;
                if (this.Card.DoKeywordsContain("prefix"))
                {
                    return this.GameController.SendMessageAction(pca.ResponsibleTurnTaker.Name + " plays " + this.Card.Definition.AlternateTitle + " " + pca.CardToPlay.Title + "!", Priority.Low, cardControllerActivatingModifiers.GetCardSource());
                }
                else if (this.Card.DoKeywordsContain("suffix"))
                {
                    return this.GameController.SendMessageAction(pca.ResponsibleTurnTaker.Name + " plays " + pca.CardToPlay.Title + " " + this.Card.Definition.AlternateTitle + "!", Priority.Low, cardControllerActivatingModifiers.GetCardSource());
                }
                return null;
            };

            return this.AddTrigger<PlayCardAction>((PlayCardAction pca) => pca?.CardSource?.Card == cardControllerActivatingModifiers.Card,
                updateCardToModify, TriggerType.Hidden, TriggerTiming.Before);
        }

        protected virtual void ClearOtherValues()
        {
            return;
        }
        protected abstract ITrigger AddModifierTriggerOverride();

    }
}