using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Extensions;

/// <summary>
/// The static class has methods that extend the functionality of the ValidationResult class.
/// </summary>
public static class ValidationResultExtension
{
    /// <summary>
    /// The method returns an error dictionary based on a list of validation results.
    /// </summary>
    /// <param name="validationResults">The validation results to convert to an error dictionary.</param>
    /// <returns>A dictionary.</returns>
    public static Dictionary<string, List<string>> ToErrorDictionary(this List<ValidationResult> validationResults)
    {
        Dictionary<string, List<string>> dictionary = [];

        foreach (var result in validationResults)
        {
            if (result.ErrorMessage is not null)
            {
                foreach (var memberName in result.MemberNames)
                {
                    if (dictionary.TryGetValue(memberName, out List<string>? value))
                    {
                        value.Add(result.ErrorMessage);
                    }
                    else
                    {
                        dictionary.Add(memberName, [result.ErrorMessage]);
                    }
                }
            }
        }

        return dictionary;
    }
}
