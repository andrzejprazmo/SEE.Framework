using SEE.Framework.Operation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SEE.Framework.Enums;

namespace SEE.Framework.Mappers
{

    public class Mapper
    {
        public List<ValidationResult> ValidationErrors { get; set; } = new List<ValidationResult>();

        #region internal map
        public object Map(object source, object destination)
        {
            Type sType = source.GetType();
            Type dType = destination.GetType();
            if (sType.CustomAttributes.Any(x => x.AttributeType == typeof(MapIgnoreAttribute)) || dType.CustomAttributes.Any(x => x.AttributeType == typeof(MapIgnoreAttribute)))
            {
                return destination;
            }
            var dProperties = dType.GetProperties()
                .Where(x => x.CanWrite)
                .Where(x => !x.PropertyType.GetInterfaces().Any(y => y == typeof(ICollection)))
                .Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(MapIgnoreAttribute)))
                .ToList();
            var sProperties = sType.GetProperties()
                .Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(MapIgnoreAttribute)))
                .Where(x => dProperties.Any(y => y.Name == x.Name))
                .ToList();
            foreach (var s in sProperties)
            {
                var d = dProperties.FirstOrDefault(x => x.Name == s.Name);
                if (d != null && d.PropertyType.FullName == s.PropertyType.FullName)
                {
                    if (s.PropertyType.IsValueType || s.PropertyType == typeof(string))
                    {
                        d.SetValue(destination, s.GetValue(source));
                    }
                    else if (s.PropertyType.IsClass)
                    {
                        var sVal = s.GetValue(source);
                        var dVal = d.GetValue(destination);
                        if (sVal != null)
                        {
                            if (dVal == null)
                            {
                                dVal = Activator.CreateInstance(d.PropertyType);
                            }
                            d.SetValue(destination, Map(sVal, dVal));
                        }
                    }
                }
            }
            return destination;
        }

        #endregion


    }

    /// <summary>
    /// Class for mapping properties of derived object to destination entity with additional context object (i.e. database context)
    /// </summary>
    /// <typeparam name="TDestination">Destination entity</typeparam>
    /// <typeparam name="TContext">Additional context object (i.e. database context)</typeparam>
    public abstract class Mapper<TDestination, TContext> : Mapper
    {
        #region map section
        /// <summary>
        /// Custom mapping. If you want to map some properties by you way fill body of this method.
        /// </summary>
        /// <param name="context">Additional context object (i.e. database context)</param>
        /// <param name="entity">Destination entity</param>
        /// <returns>Destination entity</returns>
        abstract protected TDestination Map(TContext context, TDestination entity);

        /// <summary>
        /// Custom validation. Here you can validate data by yourself.
        /// </summary>
        /// <param name="context">Additional context object (i.e. database context)</param>
        /// <returns>Collection of validation errors</returns>
        abstract protected IEnumerable<ValidationResult> Validate(TContext context);

        /// <summary>
        /// Check if source is valid. Calls <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult})"/> and <see cref="Validate(TContext)"/> methods.
        /// </summary>
        /// <param name="context">Additional context object (i.e. database context)</param>
        /// <returns>True if is valid otherwise false</returns>
        public bool IsValid(TContext context)
        {
            Validator.TryValidateObject(this, new ValidationContext(this), ValidationErrors, true);
            ValidationErrors.AddRange(Validate(context) ?? new List<ValidationResult>());
            return ValidationErrors.Count == 0;
        }

        /// <summary>
        /// Map source to destination entity if source <see cref="IsValid"/>. Automatically map properties of the same name and type (primitive and complex) except:
        /// <list type="bullet">
        /// <item>Collections</item>
        /// <item>Properties with <see cref="MapIgnoreAttribute"/> attribute.</item>
        /// </list>
        /// </summary>
        /// <param name="context">Additional context object (i.e. database context)</param>
        /// <param name="entity">Destination entity.</param>
        /// <returns><see cref="OperationResult{T}"/> with destination entity or null if validation fails.</returns>
        public OperationResult<TDestination> MapEntity(TContext context, TDestination entity)
        {
            if (IsValid(context))
            {
                base.Map(this, entity);
                return new OperationResult<TDestination>
                {
                    Value = Map(context, entity),
                };
            }
            return new OperationResult<TDestination>
            {
                Errors = ValidationErrors,
            };
        }
        #endregion

    }

    /// <summary>
    /// Class for mapping properties of derived object to destination entity
    /// </summary>
    /// <typeparam name="TDestination">Destination entity</typeparam>
    public abstract class Mapper<TDestination> : Mapper
    {
        /// <summary>
        /// Custom mapping. If you want to map some properties by you way fill the body of this method.
        /// </summary>
        /// <param name="entity">Destination entity</param>
        /// <returns>Destination entity</returns>
        abstract protected TDestination Map(TDestination entity);

        /// <summary>
        /// Custom validation. Here you can validate data by yourself.
        /// </summary>
        /// <returns>Collection of validation errors</returns>
        abstract protected IEnumerable<ValidationResult> Validate();

        /// <summary>
        /// Check if source is valid. Calls Validator.TryValidateObject() and abstract Validate() method
        /// </summary>
        /// <returns>True if is valid otherwise false</returns>
        public bool IsValid()
        {
            Validator.TryValidateObject(this, new ValidationContext(this), ValidationErrors, true);
            ValidationErrors.AddRange(Validate() ?? new List<ValidationResult>());
            return ValidationErrors.Count == 0;
        }

        /// <summary>
        /// Map source to destination entity if source <see cref="IsValid"/>. Automatically map properties of the same name and type (primitive and complex) except:
        /// <list type="bullet">
        /// <item>Collections</item>
        /// <item>Properties with <see cref="MapIgnoreAttribute"/> attribute.</item>
        /// </list>
        /// </summary>
        /// <param name="entity">Destination entity.</param>
        /// <returns><see cref="OperationResult{T}"/> with destination entity or null if validation fails.</returns>
        public OperationResult<TDestination> MapEntity(TDestination entity)
        {
            if (IsValid())
            {
                base.Map(this, entity);
                return new OperationResult<TDestination>
                {
                    Value = Map(entity),
                };
            }
            return new OperationResult<TDestination>
            {
                Errors = ValidationErrors,
            };
        }
    }

    /// <summary>
    /// When this attribute has been set, property or class will be ignored while automatic mapping. 
    /// Then, you have to map it manually inside <see cref="Mapper{TDestination}.Map(TDestination)"/> overriden method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class MapIgnoreAttribute : Attribute { }
}
