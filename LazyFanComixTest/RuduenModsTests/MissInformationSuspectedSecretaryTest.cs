using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.MissInformation;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class MissInformationSuspectSecretaryTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(MissInformationSuspectSecretaryTurnTakerController))); // replace with your own namespace
        }

        #region Load Tests

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(miss);
            Assert.IsInstanceOf(typeof(TurnTakerController), miss);
            Assert.IsInstanceOf(typeof(MissInformationSuspectSecretaryCharacterCardController), miss.CharacterCardController);
        }

        [Test()]
        public void TestSetupWorks()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            StartGame();
            AssertNumberOfCardsInPlay((Card c) => c.IsVillain && c.IsTarget, 2);
        }

        #endregion Load Tests

        #region Victory Tests

        [Test()]
        public void TestVictory()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(miss.CharacterCard);
            DestroyCard(miss.CharacterCard);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        #endregion Victory Tests

        #region Power Tests

        [Test()]
        public void TestHeroPowerA()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            StartGame();

            Card top = PutOnDeck("AnotherRealitysDebt");
            Card bottom = PutOnDeck("ConcealedBetrayal", true);

            DecisionSelectFunctions = new int?[] { 0, 1 };
            UsePower(legacy, 1);
            UsePower(legacy, 1);

            AssertUnderCard(miss.CharacterCard, top);
            AssertUnderCard(miss.CharacterCard, bottom);

            // No new cards if empty deck.
            MoveAllCards(miss, miss.TurnTaker.Deck, miss.TurnTaker.Trash);
            UsePower(legacy, 1);
            AssertNumberOfCardsUnderCard(miss.CharacterCard, 2);
        }

        [Test()]
        public void TestHeroPowerB()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            StartGame();

            Card bottom = PutOnDeck("ConcealedBetrayal", true);

            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(miss.TurnTaker.Deck, true), new MoveCardDestination(miss.TurnTaker.Trash) };

            UsePower(legacy, 2);
            AssertOnBottomOfDeck(bottom);
            UsePower(legacy, 2);
            AssertInTrash(bottom);

            // Graceful if empty deck.
            MoveAllCards(miss, miss.TurnTaker.Deck, miss.TurnTaker.Trash);
            AssertNextMessage("There are no cards in the villain deck for Miss Information - Suspect Secretary to reveal.");
            UsePower(legacy, 2);
            AssertExpectedMessageWasShown();
        }

        [Test()]
        public void TestHeroPowerFlipped()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            StartGame();

            Card target = FindCard((Card c) => c.IsInPlay && c.IsVillain && c.IsTarget);
            Card target3 = PlayCard("InspiringPresence");
            Card target2 = PlayCard(FindCard((Card c) => c.IsVillain && c.IsTarget && c.Location == miss.TurnTaker.Deck));

            FlipCard(miss);

            Card[] under = miss.TurnTaker.Deck.Cards.Take(3).ToArray();

            DecisionSelectCards = new Card[] { under[0], under[1], target, target2, target3 };
            MoveCards(miss, under, miss.CharacterCard.UnderLocation);
            UsePower(legacy, 1);
            AssertInTrash(target);
            UsePower(legacy, 1);
            AssertInTrash(target2);
            UsePower(legacy, 1);
            AssertIsInPlay(target3);
        }

        #endregion Power Tests

        #region General Tests

        [Test()]
        public void TestCardUnderNotInPlay()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            StartGame();

            MoveCard(miss, env.DeckTopCardController.Card, miss.CharacterCard.UnderLocation);
            AssertNumberOfCardsInPlay((Card c) => c.IsEnvironment, 0);
        }

        [Test()]
        public void TestVillainTargetIndestructible()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            StartGame();

            Card target = FindCard((Card c) => c.IsInPlay && c.IsVillain && c.IsTarget);

            DestroyCard(target);
            AssertIsInPlay(target);

            FlipCard(miss);
            DestroyCard(target);
            AssertInTrash(target);
        }

        [Test()]
        public void TestVillainTargetZeroReaction()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            StartGame();

            Card target = FindCard((Card c) => c.IsInPlay && c.IsVillain && c.IsTarget);

            QuickHPStorage(target);
            DealDamage(target, target, 20, DamageType.Fire);
            QuickHPCheck(0);

            DealDamage(legacy, target, 1, DamageType.Fire);
            QuickHPCheck(-2); // Legacy's power results in extra damage.
        }

        [Test()]
        public void TestFlip()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            StartGame();

            GoToStartOfTurn(miss);
            AssertNotFlipped(miss);

            Card clue = MoveCard(miss, "IsolatedHero", miss.CharacterCard.UnderLocation);
            Card diversion = MoveCard(miss, "ThreatToThePresident", miss.CharacterCard.UnderLocation);
            Card oneshot = MoveCard(miss, "DiversionaryTactics", miss.CharacterCard.UnderLocation);
            MoveCards(miss, miss.TurnTaker.Deck.Cards.Take(3), miss.CharacterCard.UnderLocation);

            Card target = FindCard((Card c) => c.IsInPlay && c.IsVillain && c.IsTarget);
            SetHitPoints(target, 5);

            GoToStartOfTurn(miss);
            AssertFlipped(miss);

            AssertUnderCard(miss.CharacterCard, clue);
            AssertIsInPlay(diversion);
            AssertInTrash(oneshot);
            //AssertIsAtMaxHP(target);
        }

        [Test()]
        public void TestFlippedEndOfTurn()
        {
            SetupGameController("MissInformation/LazyFanComix.MissInformationSuspectSecretaryCharacter", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(miss);

            QuickHPStorage(legacy);
            GoToEndOfTurn(legacy);
            QuickHPCheck(-3);
        }

        #endregion General Tests
    }
}