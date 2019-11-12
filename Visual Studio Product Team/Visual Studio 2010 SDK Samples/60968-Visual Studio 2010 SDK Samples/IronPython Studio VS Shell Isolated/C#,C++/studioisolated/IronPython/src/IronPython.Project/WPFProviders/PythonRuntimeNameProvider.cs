using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Host;
using Microsoft.Windows.Design.Model;
using System.ComponentModel;
using System.Globalization;
using System.CodeDom.Compiler;

namespace Microsoft.Samples.VisualStudio.IronPython.Project.WPFProviders
{
    class PythonRuntimeNameProvider : RuntimeNameProvider
    {
        public override string CreateValidName(string proposal)
        {
            return proposal;
        }

        public override bool IsExistingName(string name)
        {
            //We will get uniqueness in the XAML file via the matchScope predicate.
            //In a more complete implementation, this method would verify that there isn't
            //a member in the code behind file with the given name.
            return false;
        }

        public override RuntimeNameFactory NameFactory
        {
            get { return new PythonRuntimeNameFactory(); }
        }
    }

    [Serializable]
    class PythonRuntimeNameFactory : RuntimeNameFactory
    {
        public override string CreateUniqueName(Type itemType, string proposedName, Predicate<string> matchScope, bool rootScope, RuntimeNameProvider provider)
        {
            if (null == itemType) throw new ArgumentNullException("itemType");
            if (null == matchScope) throw new ArgumentNullException("matchScope");
            if (null == provider) throw new ArgumentNullException("provider");

            string name = null;
            string baseName = proposedName;

            if (string.IsNullOrEmpty(baseName))
            {
                baseName = TypeDescriptor.GetClassName(itemType);
                int lastDot = baseName.LastIndexOf('.');
                if (lastDot != -1)
                {
                    baseName = baseName.Substring(lastDot + 1);
                }

                // Names should start with a lower-case character
                baseName = char.ToLower(baseName[0], CultureInfo.InvariantCulture) + baseName.Substring(1);
            }

            int idx = 1;
            bool isUnique = false;
            while (!isUnique)
            {
                name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseName, idx++);

                // Test for uniqueness
                isUnique = !matchScope(name);

                string tempName = name;
                name = provider.CreateValidName(tempName);

                if (!string.Equals(name, tempName, StringComparison.Ordinal))
                {
                    // RNP has changed the name, test again for uniqueness
                    isUnique = !matchScope(name);
                }

                if (isUnique && rootScope)
                {
                    // Root name scope means we have to let the RNP test for uniqueness too
                    isUnique = !provider.IsExistingName(name);
                }
            }

            return name;
        }
    }

}
