using ECARules4All_DLL.Utils;
using ECARules4All_DLL;
using System.Collections.Generic;

public class RuleSanitizer
{
    // Pattern replacements for string rules
    private static readonly Dictionary<string, string> stringPatternReplacements = new Dictionary<string, string>
    {
        {"(UnityEngine.GameObject)", ""},
        {"entity_id", ""},
        {"service", ""},
        {"target", ""},
        {"fan.", ""},
        {"xiaomi_cpa4_e35d_air_purifier", ""},
        {"sensor.netatmoeud4xr_health_index", "the Netatmoeud4xr"},
        {"turn_on", "the Air Purifier turns on"},
        {"turn_off", "the Air Purifier turns off"},
        {" platform: state;  to:", "changes health index to"},
        {"unhealthy", "unhealthy then\r\n"},
        {"Window  opens", "the Window opens"},
        {"Window  closes", "and\r\nthe Window closes"}
    };

    // Word replacements for Rule rules
    private static readonly Dictionary<string, string> ruleWordReplacements = new Dictionary<string, string>
    {
        {"==", "is"},
        {"!=", "is not"},
        {"collects-dust", "collects dust from"},
        {"removes-dust-with-broom", "removes dust from"},
        {"removes-stain-with-rag", "removes stain from"},
        {"removes-stain-with-mop", "removes stain from"},
        {"sprayBottle", ""},
        {"cleaningRag", ""},
        {"broom", ""},
        {"rag", ""},
        {"mop", ""},
        {"scottex", ""},
        {"surface", ""},
        {"dustPan", ""},
        {"human", ""},
        {"spray", ""},
        {"window", ""}
    };

    public static string SanitizeRuleString(object rule)
    {
        // Check if rule is a Rule or a string to know what kind of sanitizing to apply
        bool isRule = rule is Rule;
        string ruleString = isRule ? RuleUtils.FormatRuleLabel((Rule)rule) : (string)rule;

        if (!isRule)
        {
            // Apply replacements for pattern only present in string rules
            foreach (var pair in stringPatternReplacements)
                ruleString = ruleString.Replace(pair.Key, pair.Value);
        }

        // Split string and read it word for word
        string[] words = ruleString.Split(" ");
        string sanitizedString = "";
        int counter = 1;
        foreach (var word in words)
        {
            string sanitizedWord;

            // Trim punctuation and whitespace from the word for better matching with replacements
            var trimmedWord = word.Trim('.', ',', ';', ':', '\'', '"', '{', '}', ' ', '\r', '\n');

            // Apply word replacements for Rule rules
            if (isRule)
            {
                // When words start with ';' they are replaced with "and", unless it is the last word of the rule in which case the word is removed
                if (word.StartsWith(';'))
                    sanitizedWord = counter == words.Length ? "" : "and\r\n" + trimmedWord;
                // Keep trimmed words in other cases
                else
                    sanitizedWord = trimmedWord;

                // Apply replacements for words only present in Rule rules
                if (ruleWordReplacements.ContainsKey(sanitizedWord))
                    sanitizedWord = ruleWordReplacements[sanitizedWord];
            }
            // Keep trimmed words if the rule is a string, as the other replacements have already been applied to the whole string
            else
                sanitizedWord = trimmedWord;

            // Re-establish line break before "then" and "if" keywords (it was removed in trim)
            if (sanitizedWord == "then" || sanitizedWord == "if")
                sanitizedWord = "\r\n" + sanitizedWord;

            // Add newly sanitized word to the sanitized string
            sanitizedString += sanitizedWord;

            // Add a space after each word unless the word is empty
            if (sanitizedWord != "")
                sanitizedString += " ";

            counter++;
        }

        return sanitizedString;
    }
}
