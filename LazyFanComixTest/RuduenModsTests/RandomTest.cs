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

        [Test]
        [Repeat(10)]
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
        [Repeat(10)]
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
        [Repeat(10)]
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
        [Repeat(10)]
        public void TestMyStuffAndCompletionistGuise()
        {
            GameController gameController = SetupRandomGameController(true,
                DeckDefinition.AvailableHeroes = ModHeroes.Concat(new string[] { "Guise" }).ToArray(),
                DeckDefinition.AvailableVillains.Concat(ModVillains),
                DeckDefinition.AvailableEnvironments.Concat(ModEnvironments),
                useHeroes: new List<string>() { "Guise" });
            RunGame(gameController);
            Assert.IsTrue(gameController.ShouldIncapacitatedHeroesLoseTheGame);
        }

        [Test]
        [Repeat(10)]
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
    }
}
