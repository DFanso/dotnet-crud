using FluentValidation;
using MediatR;
using TaskManager.Application.Common;

namespace TaskManager.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (!failures.Any())
        {
            return await next();
        }

        var error = string.Join("; ", failures.Select(x => x.ErrorMessage));

        object result = typeof(TResponse) == typeof(Result)
            ? Result.Failure(error)
            : CreateGenericFailure(error);

        return (TResponse)result;
    }

    private static object CreateGenericFailure(string error)
    {
        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var method = responseType.GetMethod(nameof(Result<object>.Failure), new[] { typeof(string) });
            if (method is not null)
            {
                return method.Invoke(null, new object[] { error })!;
            }
        }

        throw new ValidationException(error);
    }
}
