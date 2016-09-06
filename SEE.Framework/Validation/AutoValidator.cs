using SEE.Framework.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework.Validation
{
    public interface ICustomValidator<TContext, TContract>
    {
        void Validate(TContext context, TContract contract, IList<ValidationResult> validationErrors);
    }

    public class AutoValidator<TContract>
    {
        public static OperationResult Validate(TContract contract)
        {
            var validationErrors = new List<ValidationResult>();
            Validator.TryValidateObject(contract, new ValidationContext(contract), validationErrors, true);
            return new OperationResult(validationErrors);
        }
        public static OperationResult Validate(TContract contract, Action<TContract, List<ValidationResult>> action)
        {
            var validationErrors = new List<ValidationResult>();
            Validator.TryValidateObject(contract, new ValidationContext(contract), validationErrors, true);
            action(contract, validationErrors);
            return new OperationResult(validationErrors);
        }
    }

    public class AutoValidator<TContext, TContract>
    {
        public static OperationResult Validate(TContext context, TContract contract, Action<TContext, TContract, List<ValidationResult>> action)
        {
            var validationErrors = new List<ValidationResult>();
            Validator.TryValidateObject(contract, new ValidationContext(contract), validationErrors, true);
            action(context, contract, validationErrors);
            return new OperationResult(validationErrors);
        }

        public static OperationResult Validate(TContext context, TContract contract, ICustomValidator<TContext, TContract> customValidator)
        {
            var validationErrors = new List<ValidationResult>();
            Validator.TryValidateObject(contract, new ValidationContext(contract), validationErrors, true);
            customValidator.Validate(context, contract, validationErrors);
            return new OperationResult(validationErrors);
        }

    }

    public class AutoValidator<TContext, TContract, TCustomValidator> where TCustomValidator : ICustomValidator<TContext, TContract>, new()
    {
        public static OperationResult Validate(TContext context, TContract contract)
        {
            var validationErrors = new List<ValidationResult>();
            Validator.TryValidateObject(contract, new ValidationContext(contract), validationErrors, true);
            var customValidator = new TCustomValidator();
            customValidator.Validate(context, contract, validationErrors);
            return new OperationResult(validationErrors);
        }

    }
}
