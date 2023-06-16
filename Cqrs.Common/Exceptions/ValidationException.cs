using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.Common.Constants;

namespace Cqrs.Common.Exceptions
{
    public class ValidationException : ExceptionBase
    {
        public IDictionary<string, string[]> Errors { get; private set; }

        public ValidationException(IEnumerable<ValidationFailure> failures, Exception exception = null)
            : base(null, "Validation Exception", ApiCode.ValidationError, "One or more validation failures have occurred.", exception)
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }
    }
}
