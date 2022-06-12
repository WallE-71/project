using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ShoppingStore.Domain.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
                return false;
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var trimmedPhoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (trimmedPhoneNumber.EndsWith("."))
                return false;
            try
            {
                return Regex.Match(trimmedPhoneNumber, @"^(\+98[900-999][0-9]{7})$").Success;

                //var pattern = @"^[\+]?[{1}]?[(]?[2-9]\d{2}[)]?[-\s\.]?[2-9]\d{2}[-\s\.]?[0-9]{4}$";
                //var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
                //return Regex.IsMatch(phoneNumber, pattern, options);
            }
            catch
            {
                return false;
            }


            //      Number: (123) 456 - 7890,       Is Valid: False
            //      Number: (123)456 - 7890,        Is Valid: False
            //      Number: 123 - 456 - 7890,       Is Valid: False
            //      Number: 123.456.7890,           Is Valid: False
            //      Number: 1234567890,             Is Valid: False
            //      Number: +31636363634,           Is Valid: False
            //      Number: 075 - 63546725,         Is Valid: False

            //      Number: (657) 278 - 2011,       Is Valid: True
            //      Number: (657)278 - 2011,        Is Valid: True
            //            Number: (657)2782011,           Is Valid: True
            //            Number: 6572782011,             Is Valid: True
            //            Number: +6572782011,            Is Valid: True
            //            Number: 657 - 278 - 2011,       Is Valid: True
            //                Number: 657 278 2011,           Is Valid: True
            //                Number: 657.278.2011,           Is Valid: True
            //                Number: 1657.278.2011,          Is Valid: True
            //                Number: +6572782011,            Is Valid: True
            //                Number: 16572782011,            Is Valid: True
            //                Number: 657 - 2782011,          Is Valid: True
        }

        public static bool IsValidColorTagsInput(this string[] colorsName)
        {
            if (colorsName.Length == 0)
                return true;

            foreach (var color in colorsName)
            {
                var trimmedColor = color.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (Regex.Match(trimmedColor, @"^([g-z])$").Success && !Regex.Match(trimmedColor, @"^(\#[A-F]|[a-f]|[0-9]){6}").Success)
                    return false;
                try
                {
                    return Regex.Match(trimmedColor, @"^(\#[A-F]|[a-f]|[0-9])*").Success;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
