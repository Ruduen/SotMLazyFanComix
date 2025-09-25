using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.ShellShock;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class ShellShockTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(ShellShockCharacterCardController))); // replace with your own namespace
    }

    protected HeroTurnTakerController ShellShock
    { get { return FindHero("ShellShock"); } }

    [Test(Description = "Basic Setup and Health")]
    public void TestModWorks()
    {
      SetupGameController("BaronBlade", "LazyFanComix.ShellShock", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(ShellShock);
      Assert.IsInstanceOf(typeof(HeroTurnTakerController), ShellShock);
      Assert.IsInstanceOf(typeof(ShellShockCharacterCardController), ShellShock.CharacterCardController);

      Assert.AreEqual(22, ShellShock.CharacterCard.HitPoints);
      AssertNumberOfCardsInDeck(ShellShock, 36);
      AssertNumberOfCardsInHand(ShellShock, 4);
    }
    #region Innate Powers
    [Test()]
    public void TestInnatePowerBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectTarget = mdp;

      QuickHPStorage(mdp);
      UsePower(ShellShock);
      QuickHPCheck(-2);
    }
    #endregion Innate Powers

    #region One Shots

    [Test()]
    public void TestOneShotRooftop()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DecisionSelectTarget = mdp;

      QuickHPStorage(mdp);
      DiscardAllCards(ShellShock);
      QuickHandStorage(ShellShock);
      PlayCard("RooftopReconnissance");
      QuickHPCheck(-2);
    }
    [Test()]
    public void TestOneShotCanvas()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      PutInTrash("ZapLad");
      Card canvas = PlayCard("CanvasTheScene");
      AssertNumberOfCardsInTrash(baron, 1);
      AssertNumberOfCardsInTrash(ShellShock, 0 + 1);
      AssertNumberOfCardsInTrash(legacy, 1);
      AssertNumberOfCardsInTrash(env, 1);
      AssertIsInPlay("ZapLad");

      ShuffleTrashIntoDeck(baron);
      ShuffleTrashIntoDeck(ShellShock);
      ShuffleTrashIntoDeck(legacy);
      ShuffleTrashIntoDeck(env);

      Card top1 = PutOnDeck("MobileDefensePlatform");
      PlayCard(canvas);
      AssertOnTopOfDeck(top1);
      AssertNumberOfCardsInTrash(baron, 1 - 1);
      AssertNumberOfCardsInTrash(ShellShock, 1 + 1);
      AssertNumberOfCardsInTrash(legacy, 1);
      AssertNumberOfCardsInTrash(env, 1);
    }

    [Test()]
    public void TestOneShotMeasured()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      Card mdp = GetCardInPlay("MobileDefensePlatform");
      DealDamage(mdp, mdp, 5, DamageType.Melee);

      DecisionSelectCards = new Card[] { mdp, mdp, baron.CharacterCard, null };
      QuickHPStorage(baron);
      PlayCard("MeasuredAssault");
      AssertInTrash(mdp);
      QuickHPCheck(-1);

    }

    [Test()]
    public void TestOneShotLeft()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DealDamage(ShellShock, ShellShock, 10, DamageType.Melee);
      DealDamage(legacy, legacy, 10, DamageType.Melee);

      QuickHandStorage(ShellShock, legacy);
      QuickHPStorage(ShellShock, legacy);

      PlayCard("LeftUnspoken");

      QuickHandCheck(1, 1);
      QuickHPCheck(4, 4);

      DestroyCard(legacy.CharacterCard);

      PlayCard("LeftUnspoken");
    }
    #endregion One Shots


    #region Targets

    [Test()]
    public void TestTargetZap()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      PlayCard("ZapLad");
      Card vehicle = PlayCard("RefurbishedBlimp");
      DealDamage(vehicle, vehicle, 5, DamageType.Melee);

      QuickHPStorage(baron.CharacterCard, vehicle);
      GoToStartOfTurn(ShellShock);
      GoToEndOfTurn(ShellShock);
      QuickHPCheck(-1, 1);
    }

    [Test()]
    public void TestTargetBoom()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("ShellCopter");
      PlayCard("ShellBuggie");
      AssertInTrash(vehicle);
    }

    [Test()]
    public void TestTargetCopter()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("ShellCopter");

      QuickHPStorage(baron.CharacterCard, ShellShock.CharacterCard, vehicle);
      GoToStartOfTurn(ShellShock);
      UsePower(ShellShock, 1);
      UsePower(legacy, 1);
      QuickHPCheck(-2 - 2, - 2 - 2, -3);
    }

    [Test()]
    public void TestTargetBuggie()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("ShellBuggie");


      QuickHPStorage(baron.CharacterCard, vehicle);
      GoToStartOfTurn(ShellShock);
      UsePower(ShellShock, 1);
      UsePower(legacy, 1);
      QuickHPCheck(-3 - 3, -3);
    }

    [Test()]
    public void TestTargetGlider()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("ShellGlider");

      QuickHPStorage(ShellShock);
      DealDamage(baron, ShellShock, 2, DamageType.Melee);
      QuickHPCheck(-2 + 1);
    }

    [Test()]
    public void TestTargetBlimp()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("RefurbishedBlimp");

      DealDamage(ShellShock, ShellShock, 4, DamageType.Melee);
      DealDamage(legacy, legacy, 4, DamageType.Melee);

      QuickHandStorage(ShellShock, legacy);
      QuickHPStorage(ShellShock, legacy);

      UsePower(ShellShock, 1);
      UsePower(legacy, 1);

      QuickHandCheck(1, 1);
      QuickHPCheck(1, 1);
    }
    #endregion Targets

    #region Persistent

    [Test()]
    public void TestPersistentVehicularDefense()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card vehicle = PlayCard("ShellCopter");
      Card persistent = PlayCard("VehicularDefense");

      QuickHPStorage(baron.CharacterCard, vehicle);
      DealDamage(baron.CharacterCard, vehicle, 2, DamageType.Melee);
      DealDamage(baron.CharacterCard, vehicle, 0, DamageType.Melee);
      QuickHPCheck(-2 - 0, -2 - 0);

      DecisionSelectCard = PutInHand("RefurbishedBlimp");
      UsePower(persistent);
      AssertIsInPlay(DecisionSelectCard);
    }


    [Test()]
    public void TestPersistentPlans()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card persistent = PlayCard("PlansAndPayoff");

      QuickHPStorage(baron.CharacterCard);
      Card vehicle = PlayCard("ShellCopter");
      QuickHPCheck(-2);

      Card boom = PlayCard("BacklashField");
      UsePower(persistent);
      QuickHPCheck(-2 - 2);
      AssertInTrash(boom);
    }

    [Test()]
    public void TestPersistentEmergency()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.ShellShock", "Legacy", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card persistent = PlayCard("EmergencyLanding");

      Card vehicle = PlayCard("ShellCopter");
      DealDamage(ShellShock, ShellShock, 5, DamageType.Cold);

      QuickHandStorage(ShellShock);
      QuickHPStorage(baron.CharacterCard);
      UsePower(persistent);
      QuickHPCheck(-5);
      QuickHandCheck(2);
      AssertInTrash(vehicle);
    }

    [Test()]
    public void TestPersistentKnife()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "Legacy","LazyFanComix.ShellShock", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card persistent = PlayCard("KnifeFight");

      QuickHPStorage(baron, ShellShock);
      DealDamage(baron, ShellShock, 2, DamageType.Melee);
      UsePower(persistent);
      QuickHPCheck(-1 - 1, -2 + 1 - 1);
    }

    [Test()]
    public void TestPersistentWeeds()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "Legacy","LazyFanComix.ShellShock", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card persistent = PlayCard("InTheWeeds");
      Card vehicle = PlayCard("ShellCopter");

      QuickHPStorage(baron);
      DealDamage(vehicle, baron.CharacterCard, 1, DamageType.Melee);
      DestroyCard(vehicle);
      DealDamage(ShellShock, baron, 1, DamageType.Melee);
      // 1 Vehicle, 1 Reaction, 1 boosted by vehicle, 1 unboosted.
      QuickHPCheck(-1 - 1 - 1 - 1);
    }

    [Test()]
    public void TestPersistentEye()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade",  "Legacy","LazyFanComix.ShellShock", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card persistent = PlayCard("EyeFromAbove");

      QuickHPStorage(baron);
      PlayCard("BladeBattalion");
      QuickHPCheck(-2);
    }
    #endregion Persistent
  }
}