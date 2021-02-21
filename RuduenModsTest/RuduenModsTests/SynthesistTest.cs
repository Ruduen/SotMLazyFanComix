using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Synthesist;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class SynthesistTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(SynthesistCharacterCardController))); // replace with your own namespace
        }

        private HeroTurnTakerController synthesist { get { return FindHero("Synthesist"); } }
        private Card frame { get { return GetCard("FrameOfIronCharacter"); } }
        private Card flesh { get { return GetCard("FleshOfMercuryCharacter"); } }
        private Card heart { get { return GetCard("HeartOfLightningCharacter"); } }
        private Card[] multiChars { get { return new Card[] { frame, flesh, heart }; } }

        [Test(Description = "Basic Setup and Health")]
        public void Test1ModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Synthesist", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(synthesist);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), synthesist);
            Assert.IsInstanceOf(typeof(SynthesistCharacterCardController), synthesist.CharacterCardController);

            Assert.AreEqual(20, synthesist.CharacterCard.HitPoints);
            // TODO: Add Synthesist card count checks.
        }

        #region Character Triggers

        [Test()]
        public void TestSetup()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            // Confirm all three relics are in their flipped state. 
            AssertFlipped(multiChars);
            AssertIsInPlay(multiChars);
        }


        [Test()]
        public void TestStartOfTurnTriggerMain()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card[] cards = new Card[] { frame, heart };
            DecisionSelectCards = cards;

            GoToStartOfTurn(synthesist);
            AssertNotFlipped(cards[0]);

            GoToStartOfTurn(synthesist);
            AssertNotFlipped(cards[0]);
            AssertFlipped(cards[1]);

            DestroyCard(cards[0]);
            GoToStartOfTurn(synthesist);
            AssertUnderCard(synthesist.CharacterCard, cards[0]);
            AssertNotFlipped(cards[1]);
        }

        [Test()]
        public void TestStartOfTurnTriggerMainIncapped()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card[] cards = FindCardsWhere((Card c) => c.IsRelic && c.Owner == synthesist.HeroTurnTaker).ToArray();

            GoToStartOfTurn(synthesist);

            DestroyCard(synthesist);
            AssertIncapacitated(synthesist);

            AssertNumberOfCardsUnderCard(synthesist.CharacterCard, 3);

            GoToUseIncapacitatedAbilityPhase(synthesist);
        }

        [Test()]
        public void TestStartOfTurnTriggerScatteredForm()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist/RuduenWorkshop.SynthesistScatteredFormCharacter", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card[] cards = multiChars;
            DecisionSelectCards = cards;

            AssertNotIncapacitatedOrOutOfGame(synthesist);

            AssertIsInPlay(cards);
            AssertFlipped(cards);
            GoToStartOfTurn(synthesist);
            AssertNotFlipped(cards[0]);
            AssertFlipped(cards[1]);

            GoToStartOfTurn(synthesist);
            AssertNotFlipped(cards[0]);
            AssertFlipped(cards[1]);

            DestroyCard(cards[0]);
            GoToStartOfTurn(synthesist);
            AssertUnderCard(synthesist.CharacterCard, cards[0]);
            AssertNotFlipped(cards[1]);

            DestroyCard(cards[1]);
            GoToStartOfTurn(synthesist);

            DestroyCard(cards[2]);
            AssertIncapacitated(synthesist);

            GoToUseIncapacitatedAbilityPhase(synthesist); // Confirm this does not loop. 
        }

        #endregion Character Triggers

        #region Innate Powers

        [Test()]
        public void TestInnatePowerChannelPower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { flesh, legacy.CharacterCard, mdp };

            GoToUsePowerPhase(synthesist);

            QuickHPStorage(mdp, legacy.CharacterCard);
            UsePower(synthesist);
            QuickHPCheck(-3, -1);
        }

        //[Test()]
        //public void TestInnatePowerScatteredFormNoRitual()
        //{
        //    IEnumerable<string> setupItems = new List<string>()
        //    {
        //        "BaronBlade",  "RuduenWorkshop.Synthesist/RuduenWorkshop.SynthesistScatteredFormCharacter", "Legacy", "Megalopolis"
        //    };
        //    SetupGameController(setupItems);

        //    StartGame();

        //    Card relic = flesh;

        //    DecisionSelectCard = relic;

        //    GoToStartOfTurn(synthesist);

        //    GoToUsePowerPhase(synthesist);

        //    AssertNextMessages(new string[] { "Reduce damage dealt to synthesist's Targets by 1.", "There are no rituals with Ritual Tokens in play." });
        //    UsePower(synthesist);

        //    QuickHPStorage(relic);
        //    DealDamage(relic, relic, 3, DamageType.Infernal);
        //    QuickHPCheck(-2); // DR 1 Applied.
        //}

        #endregion Innate Powers

        #region Targets


        [Test()]
        public void TestTargetHeartOfLightning()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card relic = heart;

            DecisionSelectCard = relic;

            GoToStartOfTurn(synthesist);

            QuickHPStorage(relic);
            DealDamage(relic, relic, 3, DamageType.Infernal);
            QuickHPCheck(-4); // +1 Damage Dealt
        }

        [Test()]
        public void TestTargetBoneOfIron()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card relic = frame;

            DecisionSelectCard = relic;

            GoToStartOfTurn(synthesist);

            QuickHPStorage(relic);
            DealDamage(relic, relic, 3, DamageType.Infernal);
            QuickHPCheck(-2); // -1 Damage Dealt
        }


        [Test()]
        public void TestTargetVialOfMercury()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card relic = flesh;

            DecisionSelectCard = relic;

            GoToPlayCardPhase(synthesist);
            AssertPhaseActionCount(2);
        }
        #endregion Targets
    }
}