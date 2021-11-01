using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Collections.Generic;
using LazyFanComix;
using LazyFanComix.BreachMage;
using LazyFanComix.Cassie;
using LazyFanComix.Greyhat;
using LazyFanComix.Inquirer;
using LazyFanComix.Recall;
using LazyFanComix.Spellforge;
using LazyFanComix.Soulbinder;
using LazyFanComix.Trailblazer;
using System.Reflection;

namespace LazyFanComixTestRandom
{
    [TestFixture()]
    public class RandomTest : RandomGameTest
    {
        static string[] ModHeroes = {
            "LazyFanComix.BreachMage",
            "LazyFanComix.Cassie",
            "LazyFanComix.Greyhat",
            "LazyFanComix.Inquirer",
            "LazyFanComix.Recall",
            "LazyFanComix.Spellforge",
            "LazyFanComix.Soulbinder",
            "LazyFanComix.Trailblazer"
        };
        static string[] ModVillains = { };
        static string[] ModEnvironments = { };
        static string[] ModPromos = { }; // TODO: Add mod promos!

        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TrailblazerCharacterCardController))); // replace with your own namespace
        }


        [Test]
        [Repeat(100)]
        public void TestRandomGameWithModsToCompletion()
        {
            GameController gameController = SetupRandomGameController(false,
                DeckDefinition.AvailableHeroes.Concat(ModHeroes),
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments));
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(100)]
        public void TestSomewhatReasonableGameWithModsToCompletion()
        {
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes.Concat(ModHeroes),
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments));
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(100)]
        public void TestMyStuff()
        {
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes = ModHeroes,
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments));
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(100)]
        public void TestMyStuffAndGuise()
        {
            // For various Guise checks
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes = ModHeroes.Concat(new string[] { "Guise" }).ToArray(),
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments),
                useHeroes: new List<string>() { "Guise" });
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(100)]
        public void TestMyStuffAndTempest()
        {
            // For PW Tempest Checks
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes = ModHeroes.Concat(new string[] { "Tempest" }).ToArray(),
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments),
                useHeroes: new List<string>() { "Tempest" });
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(100)]
        public void TestMyStuffTribunal()
        {
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes = ModHeroes,
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments = new string[] { "TheCelestialTribunal" });
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }


        //[Test]
        //public void TestMyStuff()
        //{
        //    GameController gameController = SetupRandomGameController(false,
        //        availableVillains: ModVillains,
        //        availableEnvironments: ModEnvironments,
        //        useHeroes: new List<string> { "Workshopping.MigrantCoder" });
        //    RunGame(gameController);
        //}


        #region Arbitrary Tests
        // Doesn't work - arbitrary tests don't take the same actions, so we can only try to narrow down items via logs. 
        //[Test]
        //public void TestArbitrary()
        //{
        //    DeckDefinition.AvailableHeroes.Concat(ModHeroes);
        //    GameController gameController = SetupGameController(new List<String>() { "IronLegacy", "Guise", "LazyFanComix.BreachMage", "LazyFanComix.Trailblazer", "TheTempleOfZhuLong" }, true, randomSeed: 263719487);
        //    RunGame(gameController);
        //}
        #endregion Arbitrary Tests
    }
}
