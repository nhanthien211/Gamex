using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GamexService.Utilities
{
    public class StartDateAttribute :ValidationAttribute
    {
        public override bool IsValid(object value)
        {
//            var date = value as DateTime?;
            return IsValidStartDate((DateTime)value);
        }

        private bool IsValidStartDate(DateTime date)
        {
            var now = DateTime.Now.Date;
            var startDate = date.Date;
            return startDate >= now;
        }
    }

    public class EndDateAttribute : ValidationAttribute
    {
        public string StartDateProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo startDateProperty = validationContext.ObjectType.GetProperty(StartDateProperty);

            DateTime startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance, null);

            if ((DateTime) value > startDate)
            {
                return ValidationResult.Success;
            }

            // Do comparison
            // return ValidationResult.Success; // if success
            return new ValidationResult(ErrorMessage); // if fail
        }
    }
}
