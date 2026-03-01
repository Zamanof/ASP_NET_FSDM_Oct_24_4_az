namespace ASP_NET_21._TaskFlow_CQRS_Unit_tests.Basic;

public class CalculatorTests
{
    // AAA
    // Arrange
    // Act
    // Assert

    public static IEnumerable<object[]> AddData()
    {
        yield return new object[] { 1, 4, 5 };
        yield return new object[] { 7, 4, 11 };
        yield return new object[] { -1, -4, -5 };
        yield return new object[] { 17, -4, 13 };
        yield return new object[] { 10, 41, 51 };
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 10, 40, 50 };
    }

    [Fact]
    public void Add_ZeroPlusZero_ReturnsZero()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(0, 0);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Add_ZeroPlusOther_ReturnsNotZero()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(0, 2);

        // Assert
        Assert.NotEqual(0, result);
    }

    [Theory]
    //[InlineData(1, 4, 5)]
    //[InlineData(7, 4, 11)]
    //[InlineData(-1, -4, -5)]
    //[InlineData(17, -4, 13)]
    //[InlineData(10, 41, 51)]
    [MemberData(nameof(AddData))]
    public void Add_ReturnResult(int left, int right, int exceptResult)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(left, right);

        // Assert
        Assert.Equal(exceptResult, result);
    }
}
