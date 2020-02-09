using CRMS_COMAPI;
using NUnit.Framework;

namespace CrmsDotNetSamples
{
    [TestFixture]
    public class TestCases
    {
        [Test]
        public void GivenIncorrectCredentials_WhenTestLink_ExpectFailedInMessage()
        {
            COMAPIGateway gateway = new COMAPIGateway();
            gateway.Login("invalid_user", "invalid_password");

            var resultMessage = gateway.TestLink("Test message");

            Assert.IsTrue(resultMessage.Contains("failed"));
        }

        [Test]
        public void GivenCredentials_WhenTestLink_ExpectNoFailedInMessage()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            var resultMessage = gateway.TestLink("Test message");

            Assert.IsFalse(resultMessage.Contains("failed"));
        }

        [Test]
        public void GivenValidParameters_WhenRetrieveObservations2D_ExpectAvailableStatusAndValuesReturned()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            RetrieveObservations2DCommand command = new RetrieveObservations2DCommand();

            command.AddSubject("BHP");
            command.AddDataType("CLOSE PRICE");
            command.DateRange = "01/01/2018";

            IRetrieveObservations2DResponse response = gateway.RetrieveObservations2D(command);

            Assert.AreEqual(RetrieveResponseStatusEnum.Available, response.Status);

            string[,] expected = {
                {"", "CLOSE PRICE"},
                {"BHP", "29.57"}
            };

            Assert.AreEqual(expected, response.Values);
        }

        [Test]
        public void GivenInvalidSubject_WhenRetrieveObservations2D_ExpectSystemErrorStatus()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            RetrieveObservations2DCommand command = new RetrieveObservations2DCommand();

            command.AddSubject("INVALID_SUBEJCT");
            command.AddDataType("CLOSE PRICE");
            command.DateRange = "01/01/2018";

            IRetrieveObservations2DResponse response = gateway.RetrieveObservations2D(command);

            Assert.AreEqual(RetrieveResponseStatusEnum.SystemError, response.Status);
        }

        [Test]
        public void GivenValidParametersForFinancialStatement_WhenRetrieveObservations2D_ExpectAvailableStatusAndValuesReturned()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            RetrieveObservations2DCommand command = new RetrieveObservations2DCommand();

            command.AddSubject("BHP");

            command.AddPublisher("JPM");

            command.AddDataType("SALES");
            command.AddDataType("EBIT_REP");
            command.AddDataType("BROKER_NPAT");

            command.AddObservedPeriod("2018");
            command.AddObservedPeriod("2019");
            command.AddObservedPeriod("2020");

            command.Observations2DLayout = Observations2DLayoutEnum.datatypesByFinYearsAndPublishers;
            command.TransposeView = true;

            IRetrieveObservations2DResponse response = gateway.RetrieveObservations2D(command);

            Assert.AreEqual(RetrieveResponseStatusEnum.Available, response.Status);

            string[,] expected = {
                {"", "2018 JPM", "2019 JPM", "2020 JPM"},
                {"SALES", "45809000000", "45139000000", "43952834224.06"},
                { "EBIT_REP",  "16169000000", "15712000000", "16594015907.75"},
                {"BROKER_NPAT", "8933000000", "9466000000",  "10540132135.14"}
            };

            Assert.AreEqual(expected, response.Values);
        }

        [Test]
        public void GivenBHPFinancials_WhenStoreObservations2D_ExpectValuesStored()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            StoreObservations2DCommand command = new StoreObservations2DCommand();

            command.Observations2DLayout = Observations2DLayoutEnum.datatypesBySubjects;

            command.AddSubject("BHP");
            command.AddPublisher("GAMMA");

            command.AddDataType("EPS");
            command.AddDataType("DPS");

            command.AddObservedPeriod("2018");

            command.Currency = "USD";

            string[,] values = {
                {"1.10", ".03"}
            };

            command.SetValues(ref values);

            IStoreObservations2DResponse response = gateway.StoreObservations2D(command);

            Assert.AreEqual(0, response.ErrorsTotal);
        }

        [Test]
        public void GivenMarketDataForMultipleSecurities_WhenStoreObservations2D_ExpectValuesStored()
        {
            COMAPIGateway gateway = new COMAPIGateway();

            StoreObservations2DCommand command = new StoreObservations2DCommand();

            command.Observations2DLayout = Observations2DLayoutEnum.datatypesBySubjects;

            command.AddSubject("BHP");
            command.AddSubject("NAB");
            command.AddSubject("WOW");

            command.AddDataType("CLOSE PRICE");

            command.Currency = "AUD";

            string[,] values = {
                {"45"},
                {"20"},
                {"15"}
            };

            command.SetValues(ref values);

            IStoreObservations2DResponse response = gateway.StoreObservations2D(command);

            Assert.AreEqual(0, response.ErrorsTotal);
        }


    }
}
