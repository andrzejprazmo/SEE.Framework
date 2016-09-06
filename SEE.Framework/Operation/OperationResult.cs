using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework.Operation
{
    public class OperationResult
    {
        public OperationResult() { }

        public OperationResult(IEnumerable<ValidationResult> errors)
        {
            Errors = errors;
        }
        public OperationResult(ValidationResult error)
        {
            Errors = new List<ValidationResult> { error };
        }
        public OperationResult(Exception e)
        {
            Exception = e;
        }

        public Exception Exception
        {
            set
            {
                Errors = new List<ValidationResult>
                {
                    new ValidationResult(value.Message, new string[] { "SYSTEM" })
                };
            }
        }

        /// <summary>
        /// Errors collection if operation failed.
        /// </summary>
        public IEnumerable<ValidationResult> Errors { get; set; }

        /// <summary>
        /// True if no errors
        /// </summary>
        public bool Succeeded
        {
            get
            {
                return Errors == null || Errors.Count() == 0;
            }
        }

        /// <summary>
        /// Helper method to return array of <see cref="ValidationError"/>.
        /// </summary>
        /// <returns>Array of <see cref="ValidationError"/>.</returns>
        public ValidationError[] ErrorsToArray()
        {
            return Errors.SelectMany(x => x.MemberNames.Select(y => new ValidationError { MemberName = y, ErrorMessage = x.ErrorMessage })).ToArray();
        }

    }
    /// <summary>
    /// Universal class for determine operation results.
    /// </summary>
    /// <typeparam name="T">User defined object to return</typeparam>
    public class OperationResult<T> : OperationResult
    {
        public OperationResult() {}
        public OperationResult(IEnumerable<ValidationResult> errors)
            :base(errors)
        {
            Errors = errors;
        }
        public OperationResult(ValidationResult error)
            :base(error)
        {
            Errors = new List<ValidationResult> { error };
        }
        public OperationResult(Exception e)
            :base(e)
        {
            Exception = e;
        }
        public OperationResult(T value)
        {
            Value = value;
        }
        public T Value { get; set; }

    }

    /// <summary>
    /// Class for storing error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Name of the field causing error.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

}
