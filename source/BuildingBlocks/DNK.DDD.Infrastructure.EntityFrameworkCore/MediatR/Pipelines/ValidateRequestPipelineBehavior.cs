using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MediatR.Pipelines;
public class ValidateRequestPipelineBehavior<TRequest, TResponse>(IValidator<TRequest> validator, IOptions<JsonOptions> jsonOptions) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IValidator<TRequest> _validator = validator;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var canValidate = this._validator.CanValidateInstancesOfType(request.GetType());

        if (canValidate)
        {
            var validateResult = await this._validator.ValidateAsync(request, cancellationToken);

            if (!validateResult.IsValid)
            {
                var errors = validateResult.Errors.Select(_ => _.ErrorMessage).ToList();
                throw new ValidationException(JsonSerializer.Serialize(errors, this._jsonSerializerOptions));
            }
        }

        return await next();
    }
}