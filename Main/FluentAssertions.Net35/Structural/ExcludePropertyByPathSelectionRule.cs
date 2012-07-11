using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions.Common;

namespace FluentAssertions.Structural
{
    /// <summary>
    /// Selection rule that removes a particular property from the structural comparison.
    /// </summary>
    public class ExcludePropertyByPathSelectionRule : ISelectionRule
    {
        private readonly string propertyPathToExclude;

        public ExcludePropertyByPathSelectionRule(string propertyPathToExclude)
        {
            this.propertyPathToExclude = propertyPathToExclude;
        }

        /// <summary>
        /// Adds or removes properties to/from the collection of subject properties that must be included while
        /// comparing two objects for structural equality.
        /// </summary>
        /// <param name="properties">
        /// A collection of properties that was prepopulated by other selection rules. Can be empty.</param>
        /// <param name="info">
        /// Type info about the subject.
        /// </param>
        /// <returns>
        /// The collection of properties after applying this rule. Can contain less or more than was passed in.
        /// </returns>
        public IEnumerable<PropertyInfo> SelectProperties(IEnumerable<PropertyInfo> properties, TypeInfo info)
        {
            string propertyPath = info.PropertyPath;
            if (!ContainsIndexingQualifiers(propertyPathToExclude))
            {
                propertyPath = RemoveInitialIndexQualifier(propertyPath);
            }

            return properties.Where(pi => propertyPathToExclude != propertyPath.Combine(pi.Name)).ToArray();
        }

        private bool ContainsIndexingQualifiers(string path)
        {
            return path.Contains("[") || path.Contains("]");
        }

        private string RemoveInitialIndexQualifier(string propertyPath)
        {
            var indexQualifierRegex = new Regex(@"^\[\d+]");

            if (!indexQualifierRegex.IsMatch(propertyPathToExclude))
            {
                var match = indexQualifierRegex.Match(propertyPath);
                if (match.Success)
                {
                    propertyPath = propertyPath.Substring(match.Length);
                }
            }

            return propertyPath;
        }
    }
}