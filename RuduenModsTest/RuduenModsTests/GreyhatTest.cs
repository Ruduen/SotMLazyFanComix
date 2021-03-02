using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Greyhat;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class GreyhatTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(GreyhatCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Greyhat { get { return FindHero("Greyhat"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Greyhat", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Greyhat);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Greyhat);
            Assert.IsInstanceOf(typeof(GreyhatCharacterCardController), Greyhat.CharacterCardController);

            Assert.AreEqual(23, Greyhat.CharacterCard.HitPoints);
            //AssertNumberOfCardsInDeck(Greyhat, 36);
            //AssertNumberOfCardsInHand(Greyhat, 4);
        }

        [Test()]
        public void TestInnatePowerDialUp()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            QuickHandStorage(Greyhat);
            UsePower(Greyhat);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(Greyhat, 1);
        }

        #region Ungrouped Cards

        [Test()]
        public void TestPlayCoercedUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = mdp;

            QuickHPStorage(mdp);
            Card link = PlayCard("CoercedUplink");
            QuickHPCheck(-1);
            AssertNextToCard(link, mdp);
        }

        [Test()]
        public void TestPlayDigitalUplink()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            Card link = PlayCard("DigitalUplink");
            AssertNextToCard(link, legacy.CharacterCard);
            AssertNumberOfUsablePowers(legacy, 0); // Power was used.
        }

        [Test()]
        public void TestPlayDirectControl()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "RuduenWorkshop.Greyhat", "Legacy", "Megalopolis"
            };
            SetupGameController(setupItems);

            StartGame();
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));
            PlayCard("DigitalUplink");
            PlayCard("CoercedUplink");
            GoToPlayCardPhase(Greyhat);

            QuickHPStorage(baron);
            PlayCard("DirectControl");
            QuickHPCheck(-2);
            AssertNumberOfUsablePowers(Greyhat, 0);
            AssertNumberOfUsablePowers(legacy, 0);

            // TODO: If necessary, add tests for bounce cases. This is using the same code as other similar areas.
        }

        #endregion Ungrouped Cards
    }
}