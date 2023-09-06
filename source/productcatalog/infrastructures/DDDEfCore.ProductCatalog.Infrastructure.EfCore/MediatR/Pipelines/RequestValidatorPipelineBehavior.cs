using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR.Pipelines;
public class RequestValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IValidator<TRequest> _validator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RequestValidatorPipelineBehavior(IValidator<TRequest> validator, IOptions<JsonOptions> jsonOptions)
    {
        this._validator = validator;
        this._jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
    }

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