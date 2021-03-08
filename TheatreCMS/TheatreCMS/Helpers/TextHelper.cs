using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;
using System.Windows;

namespace TheatreCMS.Helpers
{
    public static class TextHelper
    {
        public static string LimitCharacterCount(this string stringText, int maxChars, bool showElipses = true)
        {
            if (stringText == null) return null;

            // If the max character wanted is less then the length of the bio it will return as is.
            if (stringText.Length <= maxChars)
            {
                return stringText;
            }
            //If the max character wanted is more then the length of the text, it will return the first through the set number of characters then add elipses
            else if (stringText.Length >= maxChars && showElipses) 
            {
                return stringText.Length <= maxChars ? stringText : stringText.Substring(0, maxChars) + "...";
            }
            // if showElipses is false it will display all text until it hits max characters and display without elipses 
            else
            {
                return stringText.Length <= maxChars ? stringText : stringText.Substring(0, maxChars);
            }
        }

        public static string LimitWordCount(string stringText, int maxWordCount, bool showElipses = true )
        {
            if (stringText == null) return stringText;

            // This replaces an double spaces with sigle spaces.
            stringText = stringText.Replace("  ", " ");

            // This creates an array of elements, split is telling the code to make whatever comes after the space an element 
            //and then the code is given a number maxWordCount which will only allow that many elements in and then adds them together.
            string query = stringText.Split(' ').Take(maxWordCount).Aggregate((a, b) => a + " " + b);

            int wordCount = stringText.Split(' ').Count();

            // If max word count has not been reached text will display normally
            if (wordCount + 1 <= maxWordCount)
            {
                return stringText;
            }
            // If max word count is met and showElipses is true text will display until it hits max words, then add elipses after
            else if (wordCount >= maxWordCount && showElipses)
            {
                return query + "...";
            }
            // if elipses is false text will display until maxWordCount and nothing else
            else
            {
                return query;
            }
        }  
 
    }
}