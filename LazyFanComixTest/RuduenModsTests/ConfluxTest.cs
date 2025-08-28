using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.Conflux;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class ConfluxTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(ConfluxCharacterCardController))); // replace with your own namespace
    }

    protected HeroTurnTakerController Conflux
    { get { return FindHero("Conflux"); } }

    [Test(Description = "Basic Setup and Health")]
    public void TestModWorks()
    {
      SetupGameController("BaronBlade", "LazyFanComix.Conflux", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(Conflux);
      Assert.IsInstanceOf(typeof(HeroTurnTakerController), Conflux);
      Assert.IsInstanceOf(typeof(ConfluxCharacterCardController), Conflux.CharacterCardController);

      Assert.AreEqual(25, Conflux.CharacterCard.HitPoints);
      AssertNumberOfCardsInDeck(Conflux, 36);
      AssertNumberOfCardsInHand(Conflux, 4);
    }


    #region Innate Tests


    [Test()]
    public void TestInnatePowerBase()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      QuickHPStorage(baron);
      UsePower(Conflux);
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

      SelectFromBoxForNextDecision("LazyFanComix.ConfluxCharacter", "LazyFanComix.Conflux");
      QuickHPStorage(baron);
      PlayCard("CalledToJudgement");
      QuickHPCheck(-2);

      UsePower(FindCardInPlay("ConfluxCharacter"));
      QuickHPCheck(-2);
    }

    #endregion Innate Tests

    #region One-Shot Tests

    [Test()]
    public void TestOneShotFlarebolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] flarebolts = this.FindCardsWhere((Card c) => c.Title == "Flarebolt").ToArray();

      MoveAllCardsFromHandToDeck(Conflux);

      MoveAllCards(Conflux, Conflux.TurnTaker.Deck, Conflux.TurnTaker.OutOfGame);

      DealDamage(Conflux, Conflux, 10, DamageType.Cold);

      MoveCards(Conflux, flarebolts, Conflux.TurnTaker.Deck);

      // Three bolts. Conflux heals, baron takes one more hit. For this chain to occur, needed to have drawn.
      QuickHPStorage(Conflux, baron);
      PlayCard(flarebolts[0]);
      QuickHPCheck(0, -2 - 2 - 2 - 3);

      AssertInTrash(flarebolts);

    }

    [Test()]
    public void TestOneShotNegabolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      Card[] bolts = this.FindCardsWhere((Card c) => c.Title == "Negabolt").ToArray();

      Card mdp = this.FindCardInPlay("MobileDefensePlatform");
      DecisionSelectTargets = new Card[] { mdp, mdp, mdp, baron.CharacterCard };

      DiscardAllCards(Conflux);

      PutInHand(bolts);

      DealDamage(Conflux.CharacterCard, mdp, 2, DamageType.Cold);

      // Three bolts set MDP to 2 and destroy, then 3 to baron.
      QuickHPStorage(baron);
      PlayCard(bolts[0]);
      QuickHPCheck(-3);
      AssertInTrash(mdp);

    }

    [Test()]
    public void TestOneShotSnowbolt()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] snowbolts = this.FindCardsWhere((Card c) => c.Title == "Snowbolt").ToArray();

      DiscardAllCards(Conflux);

      PutInHand(snowbolts);

      DealDamage(Conflux, Conflux, 10, DamageType.Cold);

      // Three snowbolts. Conflux heals, baron takes one more hit. 
      QuickHPStorage(Conflux, baron);
      PlayCard(snowbolts[0]);
      QuickHPCheck(+2 + 2 + 2, -2 - 2 - 2 - 3);

    }

    [Test()]
    public void TestOneShotLightningAndThunder()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DiscardAllCards(Conflux);

      QuickHandStorage(Conflux);


      Card mdp = this.FindCardInPlay("MobileDefensePlatform");
      DecisionSelectTarget = mdp;
      DealDamage(mdp, mdp, 6, DamageType.Cold);

      // MDP at 4 health - destroy MDP, check baron.
      QuickHPStorage(baron);
      QuickHandStorage(Conflux);
      PlayCard("LightningAndThunder");
      QuickHPCheck(-1);
      QuickHandCheck(2);
      AssertInTrash(mdp);

    }

    [Test()]
    public void TestOneShotPreyAndSpray()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DiscardAllCards(Conflux);

      QuickHandStorage(Conflux);


      Card mdp = this.FindCardInPlay("MobileDefensePlatform");
      DecisionSelectTargets = new Card[] { mdp, mdp, baron.CharacterCard };
      DealDamage(mdp, mdp, 6, DamageType.Cold);

      // MDP at 4 health - destroy MDP, check baron.
      AssertNumberOfUsablePowers(Conflux, 1);
      QuickHPStorage(baron);
      PlayCard("PreyAndSpray");
      // 1 from damage, 2 from innate power.
      QuickHPCheck(-1 - 2);
      AssertInTrash(mdp);
      AssertNumberOfUsablePowers(Conflux, 0);

    }

    #endregion One-Shot Tests

    #region Persistent Tests

    [Test()]
    public void TestPersistentEchoedBlast()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] echoes = this.FindCardsWhere((Card c) => c.Title == "Echoed Blast").ToArray();

      QuickHPStorage(baron);
      PlayCard(echoes[0]);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      QuickHPCheck(-1 - 2);
      PlayCard(echoes[1]);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      QuickHPCheck(-1 - 2);

      GoToStartOfTurn(Conflux);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      QuickHPCheck(-1 - 2 - 2);

      PutInHand(echoes[0]);
      PlayCard(echoes[0]);
      DealDamage(Conflux, baron, 1, DamageType.Psychic);
      QuickHPCheck(-1 - 2);

    }

    [Test()]
    public void TestPersistentChannelEssence()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      GoToStartOfTurn(Conflux);

      QuickHandStorage(Conflux);
      DealDamage(Conflux, Conflux, 10, DamageType.Cold);

      QuickHPStorage(Conflux);
      Card essence = PlayCard("ChannelEssence");
      QuickHPCheck(3);
      GoToEndOfTurn(Conflux);
      // 1 damage type prior to play, 1 draw.
      QuickHandCheck(1);
      AssertIsInPlay(essence);

      GoToStartOfTurn(Conflux);
      DealDamage(Conflux, Conflux, 2, DamageType.Cold);
      DealDamage(Conflux, Conflux, 2, DamageType.Cold);
      DealDamage(Conflux, Conflux, 1, DamageType.Fire);
      DealDamage(Conflux, Conflux, 0, DamageType.Lightning);
      GoToEndOfTurn(Conflux);
      QuickHandCheck(2); // Two successful damage types
      AssertIsInPlay(essence);


      GoToStartOfTurn(Conflux);
      DealDamage(Conflux, Conflux, 1, DamageType.Cold);
      DealDamage(Conflux, Conflux, 1, DamageType.Fire);
      DealDamage(Conflux, Conflux, 1, DamageType.Lightning);
      GoToEndOfTurn(Conflux);
      QuickHandCheck(3); // 3 success, selfdestruct
      AssertInTrash(essence);

    }

    [Test()]
    public void TestPersistentInterceptingShot()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card[] interceptions = new Card[] {
        PlayCard("InterceptingShot", 0),
        PlayCard("InterceptingShot", 1)
      };


      QuickHPStorage(Conflux, baron);
      DealDamage(baron, Conflux, 2, DamageType.Cold);
      QuickHPCheck(0, -2 - 2);
      AssertInTrash(interceptions[0]);
      AssertIsInPlay(interceptions[1]);
      DealDamage(baron, Conflux, 6, DamageType.Cold);
      QuickHPCheck(0, -2 - 6);
      AssertInTrash(interceptions);
    }


    [Test()]
    public void TestPersistentEyeOfTheVortex()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card eye = PlayCard("EyeOfTheVortex");
      Card[] destroy = new Card[] {
        PlayCard("LivingForceField"),
        PlayCard("BacklashField")
      };

      QuickHandStorage(Conflux);
      UsePower(eye);
      QuickHandCheck(2);
      AssertInTrash(eye);
      AssertInTrash(destroy);

      PlayCard(eye);
      PlayCards(destroy);
      DestroyCard(eye);
      QuickHandCheck(0);
      AssertInTrash(destroy);
    }


    [Test()]
    public void TestPersistentExtraArms()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card arms = PlayCard("ExtraArms", 0);
      GoToUsePowerPhase(Conflux);
      AssertPhaseActionCount(2);
      PlayCard("ExtraArms", 1);
      AssertPhaseActionCount(3);

      QuickHandStorage(Conflux);
      QuickHPStorage(baron);
      UsePower(arms, 0);
      QuickHPCheck(-1);
      UsePower(arms, 1);
      QuickHPCheck(-1);
      UsePower(arms, 2);
      QuickHPCheck(0);
      QuickHandCheck(1);
    }


    [Test()]
    public void TestPersistentOverwhelmingBlast()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();

      Card boom = PlayCard("OverwhelmingBlast");
      PlayCard("LivingForceField");

      QuickHPStorage(baron);
      UsePower(boom);
      QuickHPCheck(-3);
    }



    [Test()]
    public void TestPersistentGatheredVigor()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();
      MoveAllCardsFromHandToDeck(Conflux);

      Card vigor = PlayCard("GatheredVigor");

      UsePower(Conflux, 1);
      UsePower(Conflux, 1);
      UsePower(bunker, 1);

      // Two in play, Conflux bounces one in hand due to limited rule.
      Assert.AreEqual(FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("limited")).Count(), 2);
      Assert.AreEqual(FindCardsWhere((Card c) => c.Location == Conflux.HeroTurnTaker.Hand && c.DoKeywordsContain("limited")).Count(), 1);
    }


    [Test()]
    public void TestPersistentCascadeBlast()
    {
      IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Conflux", "Bunker", "TheCelestialTribunal"
            };
      SetupGameController(setupItems);

      StartGame();

      DestroyNonCharacterVillainCards();
      Card boom = PlayCard("CascadeBlast");

      QuickHPStorage(baron);
      UsePower(Conflux);
      AssertInTrash(boom);
      QuickHPCheck(-2 - 2);

      // Bigger boom due to previous damage.
      PlayCard(boom);
      UsePower(Conflux);
      AssertInTrash(boom);
      QuickHPCheck(-2 - 4);
    }
    #endregion Persistent Tests
  }
}