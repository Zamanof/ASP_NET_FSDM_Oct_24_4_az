using ASP_NET_21._TaskFlow_CQRS.Application.Common.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace ASP_NET_21._TaskFlow_CQRS_Unit_tests.Behaviors;

public class ValidationBehaviorTests
{
    public record TestRequest: IRequest<TestResponse>;
    public record TestResponse;
    [Fact]
    public async Task Handle_WhenValidationFails_ThrowValidationException()
    {
        var validator = new Mock<IValidator<TestRequest>>();

        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Name", "Name is Required")]));

        var validators = new List<IValidator<TestRequest>> { validator.Object };

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        RequestHandlerDelegate<TestResponse> next = _ => Task.FromResult(new TestResponse());

        var request = new TestRequest();

        var act = () => behavior.Handle(request, next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();

    }

    [Fact]
    public async Task Handle_WhenNoValidators_InvokesNext()
    {
        var behaivor = new ValidationBehavior<TestRequest, TestResponse>(Array.Empty<IValidator<TestRequest>>());

        var nextCalled = false;

        RequestHandlerDelegate<TestResponse> next = _ => { nextCalled = true; return Task.FromResult(new TestResponse()); };

        var request = new TestRequest();

        var result = await behaivor.Handle(request, next, CancellationToken.None);

        nextCalled.Should().BeTrue();

        result.Should().NotBeNull();
    }
}

