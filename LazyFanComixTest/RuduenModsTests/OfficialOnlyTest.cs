using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class OfficialOnlyTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
    }

    #region Official Tests

    [Test()]
    public void TestLaComodoraRigging()
    {
      SetupGameController("BaronBlade", "LaComodora", "TheWraith", "TheCelestialTribunal");

      StartGame();

      PlayCard("TemporalRigging");
      GoToEndOfTurn(comodora);
    }

    #endregion Official Tests
  }
}