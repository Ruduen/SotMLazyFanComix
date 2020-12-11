using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.BreachMage;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class BreachMageTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(BreachMageCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController BreachMage { get { return FindHero("BreachMage"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(BreachMage);
            Assert.IsInstanceOf(typeof(BreachMageTurnTakerController), BreachMage);
            Assert.IsInstanceOf(typeof(BreachMageCharacterCardController), BreachMage.CharacterCardController);

            Assert.AreEqual(27, BreachMage.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBreachMageInitialBreachesWork()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            IEnumerable<Card> closedBreaches = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("closed breach") && c.Owner == BreachMage.HeroTurnTaker &&  c.IsInPlay);
            IEnumerable<Card> openBreaches = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("open breach", false, true) && c.Owner == BreachMage.HeroTurnTaker && c.IsInPlay);
            // Note that due to not explicitly being a mission, character, shield, or any other special cards, we must use the special check for closed/open breaches! Yes, this is a pain. Engine restrictions! 

            Assert.IsTrue(closedBreaches.Count() == 2);
            Assert.IsTrue(openBreaches.Count() == 2);

            foreach (Card c in closedBreaches)
            {
                Assert.IsFalse(c.IsFlipped);
            }
            foreach (Card c in openBreaches)
            {
                Assert.IsTrue(c.IsFlipped);
            }
        }
    }
}