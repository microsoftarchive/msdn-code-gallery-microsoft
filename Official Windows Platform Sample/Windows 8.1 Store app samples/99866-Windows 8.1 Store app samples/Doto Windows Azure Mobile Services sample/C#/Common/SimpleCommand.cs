// Copyright (c) Microsoft Corporation. All rights reserved

using System;

namespace Doto
{
    /// <summary>
    /// A representation of a simple command for 
    /// use in Doto's dialogs and Settings
    /// </summary>
    public class SimpleCommand
    {
        public SimpleCommand()
        {
        }

        public SimpleCommand(string text, Action action)
        {
            Text = text;
            Action = action;
        }

        public string Text { get; set; }
        public Action Action { get; set; }
    }
}
