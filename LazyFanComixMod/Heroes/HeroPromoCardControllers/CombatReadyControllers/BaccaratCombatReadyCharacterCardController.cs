using Cauldron.Baccarat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.Shared;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Baccarat
{
    public class BaccaratCombatReadyCharacterCardController : BaccaratCharacterCardController
    {
        public BaccaratCombatReadyCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            return this.GameController.BulkMoveCards(this.DecisionMaker, this.TurnTaker.Deck.Cards.Take(10), this.TurnTaker.Trash, responsibleTurnTaker: this.TurnTaker, cardSource: this.GetCardSource());
            // return this.GameController.DiscardTopCards(this.DecisionMaker, this.TurnTaker.Deck, 10, cardSource: this.GetCardSource());
        }

    }
}