using JiraSynchronizer.Core.Services;
using JiraSynchronizer.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace UnitTests;

public class LoggingServiceTest
{
    [Fact]
    public void Log__WriteMessageIntoLogfile__LogfileContainsTimestampCodeMessage()
    {
        // Arrange
        const string path = @".\log.txt";
        LoggingService logService = new LoggingService();
        string result;
        string expectedResult = $"[{DateTime.Now}]\t[100]\tTestlog";

        // Act
        logService.Log(LogCategory.Information, "Testlog");
        using (StreamReader sr = new StreamReader(path))
        {
            result = File.ReadLines(path).Last();
        }

        // Assert
        result.Should().Be(expectedResult);
    }
}
