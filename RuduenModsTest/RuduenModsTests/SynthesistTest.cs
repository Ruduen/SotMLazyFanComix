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

        protected HeroTurnTakerController Synthesist { get { return FindHero("Synthesist"); } }

        [Test(Description = "Basic Setup and Health")]
        public void Test1ModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Synthesist", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Synthesist);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Synthesist);
            Assert.IsInstanceOf(typeof(SynthesistCharacterCardController), Synthesist.CharacterCardController);

            Assert.AreEqual(20, Synthesist.CharacterCard.HitPoints);
            // TODO: Add Synthesist card count checks.
        }

        #region Character Triggers

        [Test()]
        public void TestStartOfTurnTriggerMain()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card[] cards = new Card[] { GetCard("BoneOfIron"), GetCard("HeartOfLightning") };
            DecisionSelectCards = cards;

            GoToStartOfTurn(Synthesist);
            AssertIsInPlay(cards[0]);

            GoToStartOfTurn(Synthesist);
            AssertIsInPlay(cards[0]);
            AssertNotInPlay(cards[1]);

            DestroyCard(cards[0]);
            GoToStartOfTurn(Synthesist);
            AssertOutOfGame(cards[0]);
            AssertIsInPlay(cards[1]);
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

            Card[] cards = FindCardsWhere((Card c) => c.IsRelic && c.Owner == Synthesist.HeroTurnTaker).ToArray();

            DestroyCard(Synthesist);

            GoToStartOfTurn(Synthesist);
            AssertNotInPlay(cards);
        }

        [Test()]
        public void TestStartOfTurnTriggerScattered()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Synthesist/RuduenWorkshop.SynthesistScatteredFormCharacter", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card[] cards = new Card[] { GetCard("BoneOfIron"), GetCard("HeartOfLightning"), GetCard("VialOfMercury") };
            DecisionSelectCards = cards;

            AssertNotIncapacitatedOrOutOfGame(Synthesist);

            GoToStartOfTurn(Synthesist);
            AssertIsInPlay(cards[0]);

            GoToStartOfTurn(Synthesist);
            AssertIsInPlay(cards[0]);
            AssertNotInPlay(cards[1]);

            DestroyCard(cards[0]);
            GoToStartOfTurn(Synthesist);
            AssertOutOfGame(cards[0]);
            AssertIsInPlay(cards[1]);

            DestroyCard(cards[1]);
            GoToStartOfTurn(Synthesist);

            DestroyCard(cards[2]);
            GoToStartOfTurn(Synthesist);
            AssertIncapacitated(Synthesist);

            GoToStartOfTurn(Synthesist);
            AssertNotInPlay(cards);
            GoToUseIncapacitatedAbilityPhase(Synthesist); // Confirm this does not loop. 
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

            DecisionSelectCards = new Card[] { GetCard("VialOfMercury"), legacy.CharacterCard, mdp };

            GoToUsePowerPhase(Synthesist);

            QuickHPStorage(mdp, legacy.CharacterCard);
            UsePower(Synthesist);
            QuickHPCheck(-3, -1);
        }

        [Test()]
        public void TestInnatePowerScatteredFormNoRitual()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "RuduenWorkshop.Synthesist/RuduenWorkshop.SynthesistScatteredFormCharacter", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card relic = GetCard("VialOfMercury");

            DecisionSelectCard = relic;

            GoToStartOfTurn(Synthesist);

            GoToUsePowerPhase(Synthesist);

            AssertNextMessages(new string[] { "Reduce damage dealt to Synthesist's Targets by 1.", "There are no rituals with Ritual Tokens in play." });
            UsePower(Synthesist);

            QuickHPStorage(relic);
            DealDamage(relic, relic, 3, DamageType.Infernal);
            QuickHPCheck(-2); // DR 1 Applied.
        }

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

            Card relic = GetCard("HeartOfLightning");

            DecisionSelectCard = relic;

            GoToStartOfTurn(Synthesist);

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

            Card relic = GetCard("BoneOfIron");

            DecisionSelectCard = relic;

            GoToStartOfTurn(Synthesist);

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

            Card relic = GetCard("VialOfMercury");

            DecisionSelectCard = relic;

            GoToPlayCardPhase(Synthesist);
            AssertPhaseActionCount(2);
        }
        #endregion Targets
    }
}