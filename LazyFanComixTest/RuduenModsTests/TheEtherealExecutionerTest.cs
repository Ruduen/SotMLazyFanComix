using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.TheEtherealExecutionerTeam;
using System.Collections.Generic;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;

namespace LazyFanComixTest
{
    [TestFixture]
    public class TheEtherealExecutionerTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TheEtherealExecutionerTeamCharacterCardController))); // replace with your own namespace
        }

        #region Load Tests


        protected TurnTakerController TheEtherealExecutioner { get { return FindVillain("TheEtherealExecutionerTeam"); } }
        protected Card Calin { get { return FindCardInPlay("Calin"); } }
        protected Card Kanya { get { return FindCardInPlay("Kanya"); } }
        protected Card Sez { get { return FindCardInPlay("Sez"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(TheEtherealExecutioner);
            Assert.IsInstanceOf(typeof(TurnTakerController), TheEtherealExecutioner);
            Assert.IsInstanceOf(typeof(TheEtherealExecutionerTeamCharacterCardController), TheEtherealExecutioner.CharacterCardController);
        }

        [Test()]
        public void TestSetupWorks()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInDeck(TheEtherealExecutioner, 20); // Default villain card count. 
        }

        #endregion Load Tests

        #region Character Card Tests

        [Test()]
        public void TestCharacterEndOfTurn()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Megalopolis");

            StartGame();

            Card card = PutOnDeck("ExploitWeakness");
            GoToEndOfTurn(TheEtherealExecutioner);
            AssertInTrash(card);
        }

        [Test()]
        public void TestCharacterEndOfTurnAdvanced()
        {
            SetupGameController(new List<string> { "LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Megalopolis" }, true);

            StartGame();

            Card[] cards = new Card[] { PutOnDeck("ExploitWeakness"), PutOnDeck("BrokenTrail") };
            GoToEndOfTurn(TheEtherealExecutioner);
            AssertInTrash(cards);
        }

        [Test()]
        public void TestCharacterRevive()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "BaronBladeTeam", "Megalopolis");

            StartGame();

            DestroyCard(TheEtherealExecutioner.CharacterCard);
            AssertIncapacitated(TheEtherealExecutioner);

            GoToEndOfTurn(TheEtherealExecutioner);
            AssertTokenPoolCount(FindTokenPool("TheEtherealExecutionerTeamCharacter", "RespawnPool"), 1);
            GoToEndOfTurn(TheEtherealExecutioner);
            AssertTokenPoolCount(FindTokenPool("TheEtherealExecutionerTeamCharacter", "RespawnPool"), 2);
            GoToEndOfTurn(TheEtherealExecutioner);
            AssertTokenPoolCount(FindTokenPool("TheEtherealExecutionerTeamCharacter", "RespawnPool"), 3);
            AssertIncapacitated(TheEtherealExecutioner);

            GoToEndOfTurn(TheEtherealExecutioner);
            AssertTokenPoolCount(FindTokenPool("TheEtherealExecutionerTeamCharacter", "RespawnPool"), 0);
            AssertNotFlipped(TheEtherealExecutioner);
            Assert.IsTrue(TheEtherealExecutioner.CharacterCard.HitPoints > 0);

            Card card = FindCardController("ExploitWeakness").Card;
            AssertNotOutOfGame(card);

            GoToPlayCardPhaseAndPlayCard(TheEtherealExecutioner, "ExploitWeakness");
            AssertInTrash(card);

            DestroyCard(TheEtherealExecutioner.CharacterCard);
            DestroyCard(baronTeam.CharacterCard);
            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        #endregion Character Card Tests

        #region Ongoing Card Tests

        [Test()]
        public void TestObservationDisruptiveFlash()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Haka", "Megalopolis");

            StartGame();

            // 1 damage to lowest, Ob (1) discard. 
            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            Card card = PlayCard("DisruptiveFlash");
            AssertIsInPlay(card);
            QuickHPCheck(-1);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(legacy, 1);

        }

        [Test()]
        public void TestObservationRefinedStrike()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Haka", "Megalopolis");

            StartGame();

            // 2 damage, 1 increased. 
            QuickHPStorage(haka);
            Card card = PlayCard("RefinedStrike");
            AssertIsInPlay(card);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestObservationPreliminaryMeasures()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Haka", "Megalopolis");

            StartGame();

            // 2 gain, 1 increased.
            QuickHPStorage(TheEtherealExecutioner);
            Card card = PlayCard("PreliminaryMeasures");
            AssertIsInPlay(card);
            QuickHPCheck(3);
        }

        [Test()]
        public void TestObservationTargetOfOpportunity()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();

            Card target = PlayCard("DisposableDefender");
            SetHitPoints(target, 1);

            Card bonusPlay = PutOnDeck("ExploitWeakness");
            Card card = PlayCard("TargetOfOpportunity");

            AssertInTrash(target);
            AssertIsInPlay(card);
            AssertInTrash(bonusPlay);
        }

        [Test()]
        public void TestObservationTargetOfOpportunityNoPlay()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();

            QuickHPStorage(luminary);
            Card bonusPlay = PutOnDeck("ExploitWeakness");
            Card card = PlayCard("TargetOfOpportunity");
            QuickHPCheck(-1);

            AssertIsInPlay(card);
            AssertInDeck(bonusPlay);
        }

        [Test()]
        public void TestObservationPredeterminedPath()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();

            Card bonusPlay = PutOnDeck("ExploitWeakness");
            Card card = PlayCard("PredeterminedPath");

            AssertIsInPlay(card);
            AssertInTrash(bonusPlay);
        }

        #endregion Ongoing Card Tests

        #region One-Shot Tests

        [Test()]
        public void TestBrokenTrailTarget()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();
            PutOnDeck("TrafficPileup");

            QuickHPStorage(TheEtherealExecutioner);

            PlayCard("BrokenTrail");

            QuickHPCheck(4);
        }

        [Test()]
        public void TestBrokenTrailNoTarget()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();
            PutOnDeck("PoliceBackup");

            QuickHPStorage(legacy);

            PlayCard("BrokenTrail");

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestBrokenTrailBoth()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();

            PutOnDeck("PoliceBackup");
            PutOnDeck("TrafficPileup");

            PutOnDeck("BrokenTrail");

            QuickHPStorage(TheEtherealExecutioner, legacy);

            PlayCard("PredeterminedPath");

            QuickHPCheck(4, -2);
        }

        [Test()]
        public void TestExploitWeakness()
        {
            SetupGameController("LazyFanComix.TheEtherealExecutionerTeam", "Legacy", "Luminary", "Megalopolis");

            StartGame();


            PutOnDeck("ExploitWeakness");

            Card ongoing = PlayCard("InspiringPresence");

            QuickHPStorage(legacy);
            PlayCard("PredeterminedPath");
            QuickHPCheck(-4);
            AssertInTrash(ongoing);
        }

        #endregion One-Shot Tests
    }
}