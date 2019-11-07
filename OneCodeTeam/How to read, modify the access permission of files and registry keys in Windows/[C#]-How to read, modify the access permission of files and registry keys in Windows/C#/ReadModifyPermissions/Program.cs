using Microsoft.Win32;
using System.IO;
using System.Security.AccessControl;

namespace ReadModifyPermissions
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        /// <summary>
        /// Adds an ACL entry on the specified file for the specified account.
        /// The permission is added to Special Permissions.
        /// </summary>
        private static void AddFilePermission(string fileName, string account, 
            FileSystemRights rights, AccessControlType controlType)
        {
            // Get a FileSecurity object that represents the current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);
            FileSystemAccessRule permission = new FileSystemAccessRule
                                                (account, rights, controlType);

            fSecurity.AddAccessRule(permission);
            File.SetAccessControl(fileName, fSecurity);
        }

        /// <summary>
        /// Removes an ACL entry on the specified file for the specified account.
        /// </summary>
        public static void RemoveFilePermission(string fileName, string account, 
            FileSystemRights rights, AccessControlType controlType)
        {
            // Get a FileSecurity object that represents the current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);
            FileSystemAccessRule permission = new FileSystemAccessRule
                                                (account, rights, controlType);

            fSecurity.RemoveAccessRule(permission);
            File.SetAccessControl(fileName, fSecurity);
        }

        /// <summary>
        /// Adds an ACL entry on the specified folder for the specified account.
        /// </summary>
        public static bool SetDirPermission(string dirPath, string account, 
            FileSystemRights rights)
        {
            FileSystemAccessRule accessRule = new FileSystemAccessRule(account, rights,
                                        InheritanceFlags.None,
                                        PropagationFlags.NoPropagateInherit,
                                        AccessControlType.Allow);
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            DirectorySecurity dirSecurity = 
                dirInfo.GetAccessControl(AccessControlSections.Access);
            bool result = false;
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, 
                                                accessRule, out result);
            if (result)
            {
                accessRule = new FileSystemAccessRule(account, rights,
                                        InheritanceFlags.ContainerInherit |
                                        InheritanceFlags.ObjectInherit,
                                        PropagationFlags.InheritOnly,
                                        AccessControlType.Allow);
                dirSecurity.ModifyAccessRule(AccessControlModification.Add, accessRule, 
                                        out result);
                if (result)
                {
                    dirInfo.SetAccessControl(dirSecurity);
                }
            }
            return result;
        }

        /// <summary>
        /// Set registry permissions on a registry key for a specified account.
        /// </summary>
        public static bool SetRegPermission(RegistryKey rootKey, string subKeyPath, 
            string account, RegistryRights rights)
        {
            bool result = false;
            RegistryAccessRule accessRule = new RegistryAccessRule(account, rights,
                                        InheritanceFlags.None,
                                        PropagationFlags.NoPropagateInherit,
                                        AccessControlType.Allow);

            using (RegistryKey key = rootKey.OpenSubKey(subKeyPath, true))
            {
                RegistrySecurity keySecurity = 
                    key.GetAccessControl(AccessControlSections.Access);

                keySecurity.ModifyAccessRule(AccessControlModification.Add, 
                                                    accessRule, out result);
                if (result)
                {
                    accessRule = new RegistryAccessRule(account, rights,
                                            InheritanceFlags.ContainerInherit |
                                            InheritanceFlags.ObjectInherit,
                                            PropagationFlags.InheritOnly,
                                            AccessControlType.Allow);

                    keySecurity.ModifyAccessRule(AccessControlModification.Add, 
                                                accessRule, out result);
                    if (result)
                    {
                        key.SetAccessControl(keySecurity);
                    }
                }
            }
            return result;
        }
    }
}