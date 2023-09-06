using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.Entity.Design.PluralizationServices;

namespace Gsmu.Api.Data
{
    public static class GrammarHelper
    {
        private static PluralizationService pluralizationService = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en"));

        public static string GetConditionallyPluralizedWord(int count, string word)
        {
            if (count > 1)
            {
                return pluralizationService.Pluralize(word);
            }
            return word;
        }

        public static string Capital(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return word;
            }
            word = word.Trim();
            return word.First().ToString().ToUpper() + String.Join("", word.ToLower().Skip(1));
        }
    }
}
