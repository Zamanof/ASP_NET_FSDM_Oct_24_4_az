using System.Text.RegularExpressions;
using FluentValidation;

namespace TaskFlow.Application.Validators;

public static class ValidationRulesExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, bool mustContainLowerCase = true, bool mustContainUpperCase = true, bool mustContainDigit = true)
    {
        return ruleBuilder.Must(p =>
        {
            if (string.IsNullOrEmpty(p)) return false;
            if (mustContainLowerCase && !Regex.IsMatch(p, @"[a-z]")) return false;
            if (mustContainUpperCase && !Regex.IsMatch(p, @"[A-Z]")) return false;
            if (mustContainDigit && !Regex.IsMatch(p, @"\d")) return false;
            return true;
        });
    }
}
