﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RemotiatR.Client.FluentValidation
{
    public static class ValidationScope
    {
        public static ValidationResult Run(Action action)
        {
            try
            {
                action();

                return new ValidationResult();
            }
            catch (Exception exception)
            {
                var result = GetValidationResult(exception);

                if (result != null) return result;

                throw;
            }
        }

        public static async Task<ValidationResult> Run(Func<Task> action)
        {
            try
            {
                await action();

                return new ValidationResult();
            }
            catch(Exception exception)
            {
                var result = GetValidationResult(exception);

                if (result != null) return result;

                throw;
            }
        }

        private static ValidationResult GetValidationResult(Exception exception)
        {
            if (exception is ValidationException validationException)
                return new ValidationResult(validationException.Errors);

            if (exception is AggregateException aggregateException)
            {
                var validationExceptions = aggregateException.Flatten()
                    .InnerExceptions
                    .Select(x => x as ValidationException)
                    .Where(x => x != null)
                    .ToArray();

                if (validationExceptions.Any())
                    return new ValidationResult(validationExceptions.SelectMany(x => x.Errors));
            }

            return null;
        }
    }
}