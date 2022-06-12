using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Domain.Common.Attributes
{
    public class UrlValidateAttribute : ValidationAttribute
    {
        public IEnumerable<string> Characters { get; }
        public UrlValidateAttribute(params string[] characters)
        {
            Characters = new List<string>(characters);
        }

        public override bool IsValid(object value)
        {
            if (value != null)
                return !Characters.Any(value.ToString().Trim().Contains);
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} نباید شامل کاراکترهای غیر مجاز فضای خالی و ({String.Join(",", Characters)}) باشد.";
        }
    }

    public class PhoneValidateAttribute : ValidationAttribute
    {
        public IEnumerable<string> Characters { get; }
        public PhoneValidateAttribute(params string[] characters)
        {
            Characters = new List<string>(characters);
        }

        public override bool IsValid(object value)
        {
            if (value != null)
                return !Characters.Any(value.ToString().Trim().Contains);
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} نباید شامل کاراکترهای غیر مجاز فضای خالی و ({String.Join(",", Characters)}) باشد.";
        }
    }
}
