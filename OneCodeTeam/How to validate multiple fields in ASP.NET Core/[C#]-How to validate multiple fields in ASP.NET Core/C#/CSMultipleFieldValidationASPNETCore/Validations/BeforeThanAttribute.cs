using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSMultipleFieldValidationASPNETCore.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BeforeThanAttribute : ValidationAttribute, IClientModelValidator
    {
        public string ComparePropertyName { get; private set; }

        public BeforeThanAttribute(string comparePropertyName)
        {
            ComparePropertyName = comparePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var comparePropertyValue = validationContext.ObjectType.GetProperty(ComparePropertyName).GetValue(validationContext.ObjectInstance);

            if (value != null && value is DateTime
                & comparePropertyValue != null && comparePropertyValue is DateTime)
            {
                if ((DateTime)value > (DateTime)comparePropertyValue)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-beforthan", errorMessage);
            MergeAttribute(context.Attributes, "data-val-beforthan-other", ComparePropertyName);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}
