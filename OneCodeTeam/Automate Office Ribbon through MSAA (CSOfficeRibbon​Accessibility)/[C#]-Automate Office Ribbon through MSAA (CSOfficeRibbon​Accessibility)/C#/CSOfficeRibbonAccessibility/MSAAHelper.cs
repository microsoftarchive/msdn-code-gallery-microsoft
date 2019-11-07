/****************************** Module Header ******************************\
Module Name:    RibbonInfoForm.cs
Project:        CSOfficeRibbonAccessibility
Copyright (c) Microsoft Corporation.

The helper functions for Microsoft Active Accessibility (MSAA).

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using Microsoft.Office.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;


namespace CSOfficeRibbonAccessibility
{
    internal class MSAAHelper
    {
        /// <summary>
        /// Gets the child accessible objects of the given container object.
        /// </summary>
        /// <param name="accContainer">
        /// The container object's IAccessible interface.
        /// </param>
        /// <returns>
        /// The child accessible objects of the given container object.
        /// </returns>
        public static IAccessible[] GetAccessibleChildren(IAccessible accContainer)
        {
            // Get the number of child interfaces that belong to this object. 
            int childNum = 0;
            try
            {
                childNum = accContainer.accChildCount;
            }
            catch (Exception ex)
            {
                childNum = 0;
                Debug.Print(ex.Message);
            }

            // Get the child accessible objects.
            IAccessible[] accObjects = new IAccessible[childNum];
            int count = 0;
            if (childNum != 0)
            {
                NativeMethods.AccessibleChildren(accContainer, 0, childNum, 
                    accObjects, ref count);
            }
            return accObjects;
        }


        /// <summary>
        /// Gets the child accessible object by name and role text.
        /// </summary>
        /// <param name="accContainer">
        /// The container object's IAccessible interface.
        /// </param>
        /// <param name="name">The name of the object</param>
        /// <param name="roleText">The role text of the object</param>
        /// <param name="ignoreInvisible">
        /// Specifies if it's required to ignore the invisible objects.
        /// </param>
        /// <returns>
        /// The accessible object in the container that match the specified name 
        /// and role. 
        /// </returns>
        public static IAccessible GetAccessibleObjectByNameAndRole(
            IAccessible accContainer, Regex name, string roleText, 
            bool ignoreInvisible)
        {
            IAccessible objToReturn = null;
            if (accContainer != null)
            {
                // Get the child accessible objects.
                IAccessible[] children = GetAccessibleChildren(accContainer);
                foreach (IAccessible child in children)
                {
                    string childName = null;
                    string childState = string.Empty;
                    string childRole = string.Empty;
                    try
                    {
                        childName = child.get_accName(0);
                        childState = GetStateText((MSAAStateConstants)child.get_accState(0));
                        childRole = GetRoleText(Convert.ToUInt32(child.get_accRole(0)));
                    }
                    catch (Exception ex)
                    {
                        // Record the error and continue.
                        Debug.Print(ex.Message);
                        continue;
                    }

                    // If the child is invisible and it's required to ignore the 
                    // invisible objects, continue to the next object.
                    if (ignoreInvisible && childState.Contains("invisible"))
                    {
                        continue;
                    }

                    // If the name and role match, return the object.
                    if (!string.IsNullOrEmpty(childName) &&
                        name.Match(childName).Success && 
                        childRole == roleText)
                    {
                        return child;
                    }

                    // Recursively look for the object among the children.
                    objToReturn = GetAccessibleObjectByNameAndRole(child, name,
                        roleText, ignoreInvisible);
                    if (objToReturn != null)
                    {
                        return objToReturn;
                    }
                }
            }
            return objToReturn;
        }


        /// <summary>
        /// Get the role text of an accesible object.
        /// </summary>
        /// <param name="role">
        /// One of the object role constants.
        /// http://msdn.microsoft.com/en-us/library/dd373608.aspx
        /// </param>
        /// <returns>The role text of an accessible object</returns>
        public static string GetRoleText(uint role)
        {
            var roleText = new StringBuilder(1024);
            NativeMethods.GetRoleText(role, roleText, (uint)roleText.Capacity);
            return roleText.ToString();
        }


        /// <summary>
        /// Get the state text of an accessible object.
        /// </summary>
        /// <param name="stateBit">
        /// One of the object state constants.
        /// http://msdn.microsoft.com/en-us/library/dd373609.aspx
        /// </param>
        /// <returns>The state text of an accessible object</returns>
        public static string GetStateText(MSAAStateConstants stateBit)
        {
            int maxLength = 1024;
            var focusableStateText = new StringBuilder(maxLength);
            var sizeableStateText = new StringBuilder(maxLength);
            var moveableStateText = new StringBuilder(maxLength);
            var invisibleStateText = new StringBuilder(maxLength);
            var unavailableStateText = new StringBuilder(maxLength);
            var hasPopupStateText = new StringBuilder(maxLength);

            if (stateBit == (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE |
                MSAAStateConstants.STATE_SYSTEM_SIZEABLE |
                MSAAStateConstants.STATE_SYSTEM_MOVEABLE))
            {
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE,
                    focusableStateText, (uint)focusableStateText.Capacity);
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_SIZEABLE,
                    sizeableStateText, (uint)sizeableStateText.Capacity);
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_MOVEABLE,
                    moveableStateText, (uint)moveableStateText.Capacity);
                return (focusableStateText + "," + sizeableStateText + "," + moveableStateText);
            }

            if (stateBit == (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE | 
                MSAAStateConstants.STATE_SYSTEM_INVISIBLE))
            {
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE,
                    focusableStateText, (uint)focusableStateText.Capacity);
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_INVISIBLE,
                    invisibleStateText, (uint)invisibleStateText.Capacity);

                return (focusableStateText + "," + invisibleStateText);
            }
            if (stateBit == (MSAAStateConstants.STATE_SYSTEM_FOCUSABLE |
                MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE))
            {
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_FOCUSABLE,
                    focusableStateText, (uint)focusableStateText.Capacity);
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE,
                    unavailableStateText, (uint)unavailableStateText.Capacity);

                return (focusableStateText + "," + unavailableStateText);
            }
            if (stateBit == (MSAAStateConstants.STATE_SYSTEM_HASPOPUP |
                MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE))
            {
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_HASPOPUP,
                    hasPopupStateText, (uint)hasPopupStateText.Capacity);
                NativeMethods.GetStateText(MSAAStateConstants.STATE_SYSTEM_UNAVAILABLE,
                    unavailableStateText, (uint)unavailableStateText.Capacity);

                return (hasPopupStateText + "," + unavailableStateText);
            }

            var stateText = new StringBuilder(maxLength);
            NativeMethods.GetStateText(stateBit, stateText, (uint)stateText.Capacity);
            return stateText.ToString();
        }


        /// <summary>
        /// Gets the list of child accesible objects that match the role text.
        /// </summary>
        /// <param name="accContainer">
        /// The container object's IAccessible interface.
        /// </param>
        /// <param name="roleText">The role text of the object</param>
        /// <param name="accObjList">
        /// The list of child accesible objects that match the role text.
        /// </param>
        /// <param name="ignoreInvisible">
        /// Specifies if it's required to ignore the invisible objects.
        /// </param>
        public static void GetAccessibleObjectListByRole(IAccessible accContainer,
            string roleText, ref List<IAccessible> accObjList, bool ignoreInvisible)
        {
            if (accContainer != null)
            {
                // Get the child accessible objects.
                IAccessible[] children = GetAccessibleChildren(accContainer);
                foreach (IAccessible child in children)
                {
                    // Get each child's name, state and role.
                    string childRole = null;
                    string childState = string.Empty;

                    try
                    {
                        childRole = GetRoleText(Convert.ToUInt32(child.get_accRole(0)));
                        childState = GetStateText((MSAAStateConstants)child.get_accState(0));
                    }
                    catch (Exception ex)
                    {
                        // Record the error and continue.
                        Debug.Print(ex.Message);
                        continue;
                    }

                    // If the child is invisible and it's required to ignore the 
                    // invisible objects, continue to the next object.
                    if (ignoreInvisible && childState.Contains("invisible"))
                    {
                        continue;
                    }

                    // If the role matches, add the object to the result list.
                    if (childRole == roleText)
                    {
                        accObjList.Add(child);
                    }

                    // Recursively look for the object among the children.
                    GetAccessibleObjectListByRole(child, roleText, ref accObjList, 
                        ignoreInvisible);
                }
            }
        }


        /// <summary>
        /// Gets the accessible object from a window handle.
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <returns>The accessible object from the window handle</returns>
        public static IAccessible GetAccessibleObjectFromHandle(IntPtr hWnd)
        {
            IAccessible objToReturn = null;
            if (hWnd != IntPtr.Zero)
            {
                Guid iid = typeof(IAccessible).GUID;
                objToReturn = NativeMethods.AccessibleObjectFromWindow(hWnd, 0, 
                    ref iid) as IAccessible;
            }
            return objToReturn;
        }
    }
}
