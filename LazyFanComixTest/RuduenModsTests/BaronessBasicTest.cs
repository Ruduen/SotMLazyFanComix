using System.Linq;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.TheBaroness;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class BaronessBasicTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TheBaronessBasicCharacterCardController)));
      ModHelper.AddAssembly("VainFacadePlaytest", Assembly.GetAssembly(typeof(VainFacadePlaytest.TheBaroness.TheBaronessTurnTakerController)));

    }
    protected TurnTakerController Baroness
    { get { return FindVillain("TheBaroness"); } }


    #region Load Tests

    [Test()]
    public void TestModWorks()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(Baroness);
      Assert.IsInstanceOf(typeof(TurnTakerController), Baroness);
      Assert.IsInstanceOf(typeof(TheBaronessBasicCharacterCardController), Baroness.CharacterCardController);
    }

    [Test()]
    public void TestSetupWorks()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      AssertIsInPlay("Vampirism");
    }

    #endregion Load Tests

    #region Victory Tests

    [Test()]
    public void TestVictory()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      FlipCard(Baroness);
      AssertNotGameOver();
      FlipCard(Baroness);
      AssertNotGameOver();

      DestroyCard(Baroness.CharacterCard);
      AssertGameOver(EndingResult.VillainDestroyedVictory);

    }

    #endregion Victory Tests

    #region Effect Tests

    [Test()]
    public void TestBaseStartOfTurn()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");
      StartGame(); // Game starts by resolving SoT.

      DestroyNonCharacterVillainCards();
      PutOnDeck("WingedTerror"); // Pick one that doesn't selfdestruct

      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 1);

      GoToStartOfTurn(Baroness);

      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 1);
      AssertFlipped(Baroness);
    }


    [Test()]
    public void TestBaseStartOfTurnFlipNoDeck()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      DestroyNonCharacterVillainCards();
      PutInTrash(Baroness, FindCardsWhere((Card c) => c.Location == Baroness.TurnTaker.Deck));

      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 0);
      AssertFlipped(Baroness);
    }

    [Test()]
    public void TestBaseEndOfTurn()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "TheWraith", "Haka", "Unity", "Megalopolis");

      StartGame(); // Game starts by resolving SoT.
      DestroyNonCharacterVillainCards();
      FlipCard(Baroness);
      Card target = PutIntoPlay("SwiftBot");
      Card nonIntrusiveScheme = PlayCard("WingedTerror");
      QuickHPStorage(legacy.CharacterCard, wraith.CharacterCard, haka.CharacterCard, unity.CharacterCard, target);

      GoToEndOfTurn(Baroness);
      QuickHPCheck(-2, -2, 0, -2, -2);
      AssertFlipped(Baroness);

      DestroyNonCharacterVillainCards();
      GoToEndOfTurn(Baroness);
      QuickHPCheck(-2, -2, 0, -2, -2);
      AssertNotFlipped(Baroness);

      PutOnDeck(Baroness, nonIntrusiveScheme);
      GoToEndOfTurn(Baroness);
      QuickHPCheck(0, 0, 0, 0, 0);
    }


    [Test()]
    public void TestBaseAdvanced()
    {
      string[] identifiers = { "VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis" };
      SetupGameController(identifiers, advanced: true);
      StartGame(); // Game starts by resolving SoT.

      DestroyNonCharacterVillainCards();

      QuickHPStorage(Baroness);
      DealDamage(Baroness, Baroness, 2, DamageType.Projectile);
      QuickHPCheck(-2);

      PutOnDeck("WingedTerror"); // Pick one that doesn't selfdestruct
      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 1);
      AssertNotFlipped(Baroness);

      PutOnDeck("VampiricStrength");
      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 2);
      AssertNotFlipped(Baroness);

      PutOnDeck("ArcaneVeins");
      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 3);
      AssertNotFlipped(Baroness);

      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 3);
      AssertFlipped(Baroness);

      DestroyNonCharacterVillainCards();
      DealDamage(Baroness, Baroness, 2, DamageType.Projectile);
      QuickHPCheck(-1);

    }


    [Test()]
    public void TestBaseChallenge()
    {
      string[] identifiers = { "VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis" };
      SetupGameController(identifiers, challenge: true);
      StartGame(); // Game starts by resolving SoT.

      DestroyNonCharacterVillainCards();

      QuickHPStorage(legacy);
      DealDamage(Baroness, legacy, 2, DamageType.Projectile);
      QuickHPCheck(-2);

      PlayCard("WingedTerror");
      DealDamage(Baroness, legacy, 2, DamageType.Projectile);
      QuickHPCheck(-2 - 1);

      PlayCard("ArcaneVeins");
      DealDamage(Baroness, legacy, 2, DamageType.Projectile);
      QuickHPCheck(-2 - 2);

    }

    #endregion General Tests
  }
}