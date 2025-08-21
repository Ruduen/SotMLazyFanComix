using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.T210;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class T210Test : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(T210CharacterCardController))); // replace with your own namespace
    }

    protected HeroTurnTakerController T210
    { get { return FindHero("T210"); } }

    [Test(Description = "Basic Setup and Health")]
    public void TestModWorks()
    {
      SetupGameController("BaronBlade", "LazyFanComix.T210", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(T210);
      Assert.IsInstanceOf(typeof(HeroTurnTakerController), T210);
      Assert.IsInstanceOf(typeof(T210CharacterCardController), T210.CharacterCardController);

      Assert.AreEqual(28, T210.CharacterCard.HitPoints);
      AssertNumberOfCardsInDeck(T210, 36);
      AssertNumberOfCardsInHand(T210, 4);
    }

    #region Innate Tests


    [Test()]
    public void TestInnatePowerBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      UsePower(T210);
      QuickHPCheck(-2);
      UsePower(T210);
      QuickHPCheck(-2);
      UsePower(T210);
      QuickHPCheck(-2 - 3);
      UsePower(T210);
      QuickHPCheck(-2);

      GoToStartOfTurn(bunker);
      UsePower(T210);
      QuickHPCheck(-2);
      UsePower(bunker);
      QuickHPCheck(0);
      UsePower(T210);
      QuickHPCheck(-2 - 3);
      UsePower(T210);
      QuickHPCheck(-2);
    }
    [Test()]
    public void TestInnatePowerTribunal()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Tempest", "Guise", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      UsePower(tempest);
      QuickHPCheck(-1);
      UsePower(tempest);
      QuickHPCheck(-1);

      SelectFromBoxForNextDecision("LazyFanComix.T210Character", "LazyFanComix.T210");
      PlayCard("CalledToJudgement");
      QuickHPCheck(-2 - 3);

      GoToStartOfTurn(guise);
      UsePower(tempest);
      QuickHPCheck(-1);
      UsePower(tempest);
      QuickHPCheck(-1);
      UsePower(FindCardInPlay("T210Character"));
      QuickHPCheck(-2 - 3);
    }

    [Test()]
    public void TestInnatePowerTeam()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210/LazyFanComix.T210TeamCharacter", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      UsePower(T210, 0);
      QuickHPCheck(-2);
      UsePower(T210, 1);
      QuickHPCheck(-1);
    }


    [Test()]
    public void TestInnatePowerChibi()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210/LazyFanComix.T210ChibiCharacter", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();


      QuickHPStorage(baron);
      QuickHandStorage(T210);
      UsePower(T210);
      UsePower(T210);
      UsePower(T210);
      UsePower(T210);
      QuickHPCheck(0);
      QuickHandCheck(4);

      PlayCard("LivingForceField");
      UsePower(T210);
      QuickHPCheck(-9);
      QuickHandCheck(1);


    }


    #endregion Innate Tests

    #region Equipment Tests

    [Test()]
    public void TestLoadoutKnife()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      UsePower(T210);

      Card ongoing = PlayCard("BacklashField");

      QuickHPStorage(baron);
      Card equip = PlayCard("LoadoutKnife");
      QuickHPCheck(-2);
      UsePower(equip, 0);
      QuickHPCheck(-2);
      AssertInTrash(ongoing);
      UsePower(equip);
      QuickHPCheck(-2);

      GoToStartOfTurn(T210);
      Card mdp = PlayCard("MobileDefensePlatform");
      QuickHPStorage(baron.CharacterCard, mdp);
      UsePower(equip, 0);
      QuickHPCheck(0, -2);
      UsePower(equip, 0);
      QuickHPCheck(0, -2);
      QuickHPStorage(baron);
      UsePower(equip, 0);
      QuickHPCheck(0);
      AssertInTrash(mdp);
      UsePower(equip);
      QuickHPCheck(-2);

    }

    [Test()]
    public void TestLoadoutFirestorm()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      UsePower(T210);

      Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1), PlayCard("BladeBattalion") };

      DecisionSelectTargets = new Card[] { targets[0], targets[0], targets[1], targets[2] };

      QuickHPStorage(targets);
      Card equip = PlayCard("LoadoutFirestorm");
      QuickHPCheck(-3, -0, -0);
      UsePower(equip, 0);
      QuickHPCheck(-3, -3, -3);
      UsePower(equip);
      QuickHPCheck(-3, -0, -0);

    }


    [Test()]
    public void TestLoadoutWhisper()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      UsePower(T210);

      Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1), T210.CharacterCard };

      QuickHandStorage(T210);
      QuickHPStorage(targets);
      Card equip = PlayCard("LoadoutWhisper");
      DealDamage(targets[0], T210.CharacterCard, 2, DamageType.Cold);
      QuickHPCheck(-1, -1, -2);
      QuickHandCheck(0);
      UsePower(equip);
      DealDamage(targets[0], T210.CharacterCard, 2, DamageType.Cold);
      QuickHPCheck(-1, -1, -2 + 1);
      QuickHandCheck(1);
      UsePower(equip);
      DealDamage(targets[0], T210.CharacterCard, 2, DamageType.Cold);
      QuickHPCheck(-1, -1, -2 + 1);
      QuickHandCheck(0);
    }

    [Test()]
    public void TestLoadoutDoubleUse()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card destroyed = PlayCard("LoadoutWhisper");

      Card[] targets = new Card[] { PlayCard("MobileDefensePlatform", 0), PlayCard("MobileDefensePlatform", 1), PlayCard("BladeBattalion") };

      DecisionSelectTargets = new Card[] { targets[0], targets[0], targets[1], targets[2] };

      QuickHPStorage(targets);
      Card equip = PlayCard("LoadoutFirestorm");
      // Double fire, second shot would target same twice.
      QuickHPCheck(-6, -3, -3);
      AssertIsInPlay(equip);
      AssertInTrash(destroyed);

    }

    #endregion Equipment Tests

    #region Ongoing Tests

    [Test()]
    public void TestOngoingDoubleTap()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      Card equip = PlayCard("LoadoutWhisper");
      DestroyNonCharacterVillainCards();

      GoToUsePowerPhase(T210);
      AssertPhaseActionCount(1);

      Card card = PlayCard("DoubleTap");
      AssertPhaseActionCount(2);

      GoToUsePowerPhase(T210);
      AssertPhaseActionCount(2);

      DestroyCard(card);
      AssertPhaseActionCount(1);

    }

    [Test()]
    public void TestOngoingFindTheShot()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();
      DiscardAllCards(T210);

      Card power = PlayCard("FindTheShot");
      Card equip = PlayCard("LoadoutWhisper");

      GoToStartOfTurn(T210);

      UsePower(equip);

      Card discard = PutInHand("CallTheShot");
      DecisionSelectCard = discard;


      QuickHPStorage(baron);
      QuickHandStorage(T210);
      UsePower(power);
      AssertInTrash(discard);
      QuickHPCheck(-2 - 3); // Base power used. Third power, so confirm increase.
      QuickHandCheck(0); // Draw 1, Discard 1. 
    }


    [Test()]
    public void TestOngoingCallTheShot()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card power = PlayCard("CallTheShot");

      QuickHPStorage(baron);
      QuickHandStorage(bunker);
      UsePower(power);
      UsePower(T210);
      QuickHPCheck(-2 - 3); // Confirm bonus damage from default confirmed
      QuickHandCheck(1);    // and Bunker used initialize.
    }

    [Test()]
    public void TestConfigFlashFlare()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      PlayCard("ConfigFlashFlare");

      QuickHPStorage(baron);
      GoToEndOfTurn(T210);
      QuickHPCheck(0);

      GoToStartOfTurn(T210);
      UsePower(T210);
      QuickHPCheck(-2);

      GoToEndOfTurn(T210);
      QuickHPCheck(-1);

      GoToStartOfTurn(T210);
      UsePower(T210);
      QuickHPCheck(-2);
      UsePower(T210);
      QuickHPCheck(-2);

      GoToEndOfTurn(T210);
      QuickHPCheck(-2);

    }


    [Test()]
    public void TestConfigAutoAssault()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card play = PutInHand("LoadoutFirestorm");
      DecisionSelectCardToPlay = play;
      PlayCard("ConfigAutoAssault");

      AssertNotInPlay(play);

      GoToStartOfTurn(T210);
      AssertNotInPlay(play);

      PlayCard("FindTheShot");
      GoToStartOfTurn(T210);
      AssertIsInPlay(play);
    }


    [Test()]
    public void TestConfigRapidReboot()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Bunker", "Megalopolis"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card loadout = PutIntoPlay("LoadoutFirestorm");
      PlayCard("ConfigRapidReboot");

      DealDamage(T210, T210, 5, DamageType.Cold);

      QuickHPStorage(T210);

      // Mandatory regain. 
      GoToEndOfTurn(T210);
      QuickHPCheck(1);

      DestroyCard(loadout);

      DecisionSelectFunction = 1;
      GoToEndOfTurn(T210);
      QuickHPCheck(0);
      AssertInHand(loadout);
    }

    #endregion Ongoing Tests

    #region One-Shot Tests


    [Test()]
    public void TestOneShotOptimizeWeaponry()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card power = PlayCard("LoadoutFirestorm");
      Card play = PutInTrash("OptimizeWeaponry");

      DecisionSelectPowers = new Card[]
      {
        power, T210.CharacterCard, T210.CharacterCard, power
      };

      GoToStartOfTurn(T210);

      // Special: Here only, check both ways so mandatory decision case won't come up.

      QuickHPStorage(baron);
      QuickHandStorage(T210);

      PlayCard(play);
      QuickHPCheck(-3 - 3);
      QuickHandCheck(0);


      PlayCard(play);
      QuickHPCheck(-2); // Base power, no extra damage.
      QuickHandCheck(2);

      GoToStartOfTurn(T210);

      PlayCard(play);
      QuickHPCheck(-2); // Base power, no extra damage.
      QuickHandCheck(2);

      PlayCard(play);
      QuickHPCheck(-3 - 3);
      QuickHandCheck(0);
    }

    [Test()]
    public void TestOneShotOptimizeAwareness()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card power = PlayCard("CallTheShot");
      Card play = PutInTrash("OptimizeAwareness");

      DecisionSelectPowers = new Card[]
      {
        T210.CharacterCard, ra.CharacterCard, power
      };

      GoToStartOfTurn(T210);

      QuickHandStorage(T210, ra);

      // No discard, draw 2 from effect. 
      PlayCard(play);
      QuickHandCheck(2, 0);

      // Use power by default, then use ra's power.
      // Discard 1 from power, all draw 1 from effect;
      PlayCard(play);
      QuickHandCheck(0, 1);
    }

    [Test()]
    public void TestOneShotOptimizeFrame()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      Card power = PlayCard("LoadoutFirestorm");
      Card play = PutInTrash("OptimizeFrame");

      DecisionSelectPowers = new Card[]
      {
        T210.CharacterCard, ra.CharacterCard
      };

      GoToStartOfTurn(T210);
      DealDamage(T210, T210, 10, DamageType.Cold);

      QuickHandStorage(T210);
      QuickHPStorage(T210);

      // Heal effect.
      PlayCard(play);
      QuickHandCheck(0);
      QuickHPCheck(4);

      // No discard, draw 2 from effect. 
      PlayCard(play);
      QuickHandCheck(2);
      QuickHPCheck(0);
    }

    [Test()]
    public void TestOneShotDualStrike()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      PlayCard("DualStrike");
      QuickHPCheck(-2 - 2); // Two 2 damage powers.
    }


    [Test()]
    public void TestOneShotThirdTimesTheCharm()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();
      DestroyNonCharacterVillainCards();
      DiscardAllCards(T210);

      QuickHandStorage(T210);
      QuickHPStorage(T210, baron);
      PlayCard("ThirdTimesACharm");
      QuickHandCheck(3);
      QuickHPCheck(-3, -2 - 2 - 2 - 3);
    }

    #endregion One-Shot Tests

    #region Complex Tests

    // Todo: Eventual test for Numerology on base power. Shouldn't happen with official content. 
    //[Test()]
    //public void TestGuiseNumerologyPower()
    //{
    //  SetupGameController("BaronBlade", "Guise", "TheHarpy", "LazyFanComix.T210", "TheCelestialTribunal");

    //  StartGame();

    //  DestroyNonCharacterVillainCards();

    //  PlayCard("AppliedNumerology");
    //  DecisionSelectTurnTaker = harpy.TurnTaker;
    //  PlayCard("UhYeahImThatGuy");


    //  MoveCard(T210, "T210Character", guise.HeroTurnTaker.Hand);
    //  MoveCard(T210, "T210Character", guise.TurnTaker.PlayArea);

    //  DecisionSelectPower = T210.CharacterCard;
    //  PlayCard("ICanDoThatToo");
    //}
    #endregion Complex Tests

    #region MoveUnderCards

    //// Reserved for future character idea. 
    //[Test()]
    //public void TestUnderCardCoveringFire()
    //{
    //  IEnumerable<string> setupItems = new List<string>()
    //        {
    //            "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
    //        };
    //  SetupGameController(setupItems);

    //  StartGame();
    //  DestroyNonCharacterVillainCards();

    //  QuickHPStorage(T210, ra, fanatic);
    //  PlayCard("CoveringFire");
    //  DealDamage(T210, T210, 4, DamageType.Projectile);
    //  DealDamage(T210, ra, 4, DamageType.Projectile);
    //  DealDamage(T210, ra, 4, DamageType.Projectile);
    //  DealDamage(T210, fanatic, 4, DamageType.Projectile);
    //  DealDamage(T210, fanatic, 4, DamageType.Projectile);
    //  QuickHPCheck(0, 0, 0);

    //  DealDamage(baron, T210, 4, DamageType.Projectile);
    //  DealDamage(baron, ra, 1, DamageType.Projectile);
    //  DealDamage(baron, fanatic, 5, DamageType.Projectile);

    //  QuickHPCheck(-4 + 2, -1 + 1, -5 + 4);

    //  AssertNumberOfCardsInTrash(T210, 1);
    //  AssertNumberOfCardsInTrash(ra, 2);
    //  AssertNumberOfCardsInTrash(fanatic, 2);
    //}


    //[Test()]
    //public void TestUnderCardLeadTheTarget()
    //{
    //  IEnumerable<string> setupItems = new List<string>()
    //        {
    //            "BaronBlade", "LazyFanComix.T210", "Ra", "Fanatic", "TheCelestialTribunal"
    //        };
    //  SetupGameController(setupItems);

    //  StartGame();
    //  DestroyNonCharacterVillainCards();

    //  PlayCard("LeadTheTarget");
    //  UsePower(T210);

    //  QuickHPStorage(baron);
    //  DealDamage(fanatic, baron, 2, DamageType.Fire);
    //  QuickHPCheck(-2);
    //  DealDamage(ra, baron, 2, DamageType.Fire);
    //  QuickHPCheck(-2 - 1);
    //  AssertNumberOfCardsInTrash(ra, 1);
    //  DealDamage(ra, baron, 2, DamageType.Fire);
    //  QuickHPCheck(-2);
    //}
    #endregion MoveUnderCards

  }
}