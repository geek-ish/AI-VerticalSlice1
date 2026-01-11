using CsvUpdateDemo.Features.ImportCsv;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvUpdateDemo.Tests
{
    [TestClass]
    public class ImportCsvHandlerTests
    {
        [TestMethod]
        public void Handle_ValidCsv_ReturnsTwoValidRows_NoErrors()
        {
            var csv =
                "FullName,Title,EmploymentType,Location,UserId\r\n" +
                "Ada Lovelace,Engineer,1,London,101\r\n" +
                "Grace Hopper,Rear Admiral,2,Arlington,102\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert.AreEqual(2, result.ValidRows.Count);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("Ada Lovelace", result.ValidRows[0].FullName);
        }

        [TestMethod]
        public void Handle_InvalidInt_AddsError_RowNotIncluded()
        {
            var csv =
                "FullName,Title,EmploymentType,Location,UserId\r\n" +
                "Bad Row,Engineer,notint,Somewhere,101\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert.AreEqual(0, result.ValidRows.Count);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("EmploymentType", result.Errors[0].ColumnName);
            Assert.AreEqual(1, result.Errors[0].RowNumber);
        }

        [TestMethod]
        public void Handle_MissingRequiredValue_AddsError()
        {
            var csv =
                "FullName,Title,EmploymentType,Location,UserId\r\n" +
                ",Engineer,1,London,101\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert.AreEqual(0, result.ValidRows.Count);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("FullName", result.Errors[0].ColumnName);
        }

        [TestMethod]
        public void Handle_QuotedCommaInField_ParsesCorrectly()
        {
            var csv =
                "FullName,Title,EmploymentType,Location,UserId\r\n" +
                ""Lovelace, Ada",Engineer,1,London,101\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert.AreEqual(1, result.ValidRows.Count);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("Lovelace, Ada", result.ValidRows[0].FullName);
        }

        [TestMethod]
        public void Handle_MissingRequiredHeaderColumn_ReportsHeaderError()
        {
            var csv =
                "FullName,Title,Location\r\n" +
                "Ada Lovelace,Engineer,London\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert.IsTrue(result.Errors.Count >= 1);
        }
    }
}
