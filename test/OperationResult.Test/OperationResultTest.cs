using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OperationResult.Test
{
    public class OperationResultTest
    {
        [Fact]
        public void Result_Success_Return_True()
        {
            var result = Result.Success();

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Result_Error_Return_False()
        {
            var result = Result.Error(new Exception());

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Result_Should_Throw_Argument_Null_Exception_When_Pass_Null_Exception()
        {
            Action action = () => Result.Error(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ResultT_Should_Throw_Argument_Null_Exception_When_Pass_Null_Exception()
        {
            Action action = () => Result.Error<string>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Result_Error_Return_Task_Result()
        {
            var result = Result.Error(new Exception());

            result.AsTask.Should().BeOfType<Task<Result>>();
            result.AsTask.Should().Awaiting(x =>
            {
                x.Subject.As<Result>().IsSuccess.Should().BeFalse();

                return x.Subject.As<Result>();
            });
        }

        [Fact]
        public void Result_Success_Return_Task_Result()
        {
            var result = Result.Success();

            result.AsTask.Should().BeOfType<Task<Result>>();
            result.AsTask.Should().Awaiting(x =>
            {
                x.Subject.As<Result>().IsSuccess.Should().BeTrue();

                return x.Subject.As<Result>();
            });
        }

        [Fact]
        public void Change_Type_Of_T_Using_ChangeInAnotherResult()
        {
            var value = new
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString("N")[15..]
            };

            var result = Result.Success(value).ChangeInAnotherResult(x => new TemplateClass(x.Id, x.Name));

            result.Should().BeOfType<Result<TemplateClass>>();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<TemplateClass>();
        }

        [Fact]
        public void Change_Type_Of_ResultT_To_Result()
        {
            var value = new TemplateClass
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString("N")[15..]
            };

            var result = Result.Success(value).ChangeInNoResult();

            result.Should().BeOfType<Result>();
            result.IsSuccess.Should().BeTrue();
            result.Exception.Should().BeNull();
        }

        [Fact]
        public void Deconstruct_Result()
        {
            var value = new TemplateClass
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString("N")[15..]
            };

            var result = Result.Success(value);

            var (actionResult, entity, exception) = result;

            actionResult.Should().Be(ActionResult.Success);
            entity.Should().NotBeNull();
            entity.Should().BeOfType<TemplateClass>();
            exception.Should().BeNull();

            (actionResult, entity) = result;

            actionResult.Should().Be(ActionResult.Success);
            entity.Should().NotBeNull();
            entity.Should().BeOfType<TemplateClass>();
        }

        [Fact]
        public void Result_Should_Pass_True_Or_False_In_Condition()
        {
            var result = Result.Success();

            bool isSuccess = result;

            isSuccess.Should().BeTrue();
            ((bool)result).Should().BeTrue();

            result = Result.Error(new Exception());

            bool isFailure = result;

            isFailure.Should().BeFalse();
            ((bool)result).Should().BeFalse();

            var newResult = Result.Error<string>(new Exception());

            isFailure = newResult;

            isFailure.Should().BeFalse();
            ((bool)newResult).Should().BeFalse();
        }

        [Fact]
        public void Result_Should_Cast_To_Task()
        {
            var result = Result.Success();

            Task operation = result;

            operation.GetAwaiter().IsCompleted.Should().BeTrue();
        }

        private class TemplateClass
        {
            public TemplateClass()
            {

            }

            public TemplateClass(Guid id, string name)
            {
                Id = id;
                Name = name;
            }

            public Guid Id { get; init; }
            public string Name { get; init; }
        }
    }
}
