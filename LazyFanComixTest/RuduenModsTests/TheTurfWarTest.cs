using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.TheTurfWar;
using System.Collections.Generic;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;

namespace LazyFanComixTest
{
    [TestFixture]
    public class TheTurfWarTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TheTurfWarInstructionsCardController))); // replace with your own namespace
        }

        #region Load Tests


        protected TurnTakerController TurfWar { get { return FindVillain("TheTurfWar"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(TurfWar);
            Assert.IsInstanceOf(typeof(TurnTakerController), TurfWar);
            Assert.IsInstanceOf(typeof(TheTurfWarInstructionsCardController), TurfWar.CharacterCardController);
        }

        [Test()]
        public void TestSetupWorks()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();
            AssertIsInPlay("Calin");
            AssertIsInPlay("Kanya");
            AssertIsInPlay("Sez");

            AssertNumberOfCardsInDeck(TurfWar, 20); // Default villain card count. 
        }

        #endregion Load Tests

        #region Victory Tests
        [Test()]
        public void TestPartialVictory()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            DestroyCard("Calin");
            DestroyCard("Kanya");

            GoToStartOfTurn(TurfWar);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test()]
        public void TestTrueVictory()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            DestroyCard("Calin");
            DestroyCard("Kanya");
            DestroyCard("Sez");

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }
        #endregion Victory Tests

        #region Instructions Card Test
        [Test()]
        public void TestFlippedActivation()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(TurfWar);
            
            GoToStartOfTurn(TurfWar);
        }

        [Test()]
        public void TestFlippedActivationDestroyBasic()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(TurfWar);

            Card[] cards = env.TurnTaker.Deck.Cards.ToArray();

            MoveCard(env, cards[0], FindCardInPlay("Calin").UnderLocation);
            MoveCard(env, cards[1], FindCardInPlay("Kanya").UnderLocation);
            MoveCard(env, cards[2], FindCardInPlay("Kanya").UnderLocation);
            MoveCard(env, cards[3], FindCardInPlay("Kanya").UnderLocation);

            DecisionDestroyCards = new Card[] { cards[0], cards[1], cards[2], null };

            GoToStartOfTurn(TurfWar);

            AssertInTrash(cards[0], cards[1], cards[2]);
            AssertIsInPlay(cards[3]);
        }

        [Test()]
        public void TestFlippedActivationDestroySavedFromDestruction()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            // TODO: Need to figure out the convoluted circumstance which could have something be 'not destroyed', then be destroyed, then re-played and is valid for destruction. 
            // No, I can't yet think of such a case. Yes, it's a pain.
        }

        [Test()]
        public void TestFlippedActivationLeaderDamageIsDestroyFailsDiscover()
        {
            /// Scenario where the leader is destroyed along the way. 
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Megalopolis");

            StartGame();

            Card calin = FindCardInPlay("Calin");
            SetHitPoints(calin, 2);
            PlayCard("DrivingMantis");
            DecisionRedirectTarget = calin;
            FlipCard(TurfWar);

            GoToStartOfTurn(TurfWar);

            AssertNotInPlay((Card c) => c.IsCultist);
        }


        #endregion Instructions Card Test

        #region Non-Specific Cards Test
        [Test()]
        public void TestCardTheRealmsChoice()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            FlipCard(TurfWar); // Flip so destroy trigger isn't too nuts.
            PlayCard("TheRealmsChoice");
            Card destroyTarget = PlayCard("Bloogo");

            GoToEndOfTurn(TurfWar);
            AssertInTrash(destroyTarget);
            AssertNumberOfCardsInPlay(env, 2);
        }

        #endregion Non-Specific Cards Test
    }
}