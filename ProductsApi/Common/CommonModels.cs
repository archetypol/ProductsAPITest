using System.Diagnostics.CodeAnalysis;

// Opted to use errors as values instead of exceptions
namespace ProductsApi.Common.Models
{
    public class Result<T>
    {
        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Errors))]
        public bool IsSuccess { get; }
        public T? Value { get; }
        public IEnumerable<string>? Errors { get; }

        protected Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        protected Result(IEnumerable<string> errors)
        {
            IsSuccess = false;
            Errors = errors;
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(IEnumerable<string> errors) => new(errors);
    }
}
