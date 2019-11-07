/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2012 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/
namespace ShareResources
{
    partial class ShareResources_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ClientState_Label = new System.Windows.Forms.Label();
            this.ClientStateString_Label = new System.Windows.Forms.Label();
            this.Contact_ListBox = new System.Windows.Forms.ListBox();
            this.StartSharingResource_Button = new System.Windows.Forms.Button();
            this.Accept_Button = new System.Windows.Forms.Button();
            this.Decline_Button = new System.Windows.Forms.Button();
            this.Grant_Button = new System.Windows.Forms.Button();
            this.SharingAction_GroupBox = new System.Windows.Forms.GroupBox();
            this.Disconnect_Button = new System.Windows.Forms.Button();
            this.Revoke_Button = new System.Windows.Forms.Button();
            this.Request_Button = new System.Windows.Forms.Button();
            this.Release_Button = new System.Windows.Forms.Button();
            this.SharingParticipationState_Label = new System.Windows.Forms.Label();
            this.SharingParticipationStateString_Label = new System.Windows.Forms.Label();
            this.SharedResources_ListBox = new System.Windows.Forms.ListBox();
            this.ConversationActions_Group = new System.Windows.Forms.GroupBox();
            this.EndConversation_Button = new System.Windows.Forms.Button();
            this.Start_Button = new System.Windows.Forms.Button();
            this.ResourceController_Label = new System.Windows.Forms.Label();
            this.ResourceControllerName_Label = new System.Windows.Forms.Label();
            this.ShareableResources_Label = new System.Windows.Forms.Label();
            this.ContactList_Label = new System.Windows.Forms.Label();
            this.SharedResource_Label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RejectInvitation_Button = new System.Windows.Forms.Button();
            this.AcceptInvitation_Button = new System.Windows.Forms.Button();
            this.RefreshResource_Button = new System.Windows.Forms.Button();
            this.SharingAction_GroupBox.SuspendLayout();
            this.ConversationActions_Group.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClientState_Label
            // 
            this.ClientState_Label.AutoSize = true;
            this.ClientState_Label.Location = new System.Drawing.Point(13, 13);
            this.ClientState_Label.Name = "ClientState_Label";
            this.ClientState_Label.Size = new System.Drawing.Size(64, 13);
            this.ClientState_Label.TabIndex = 0;
            this.ClientState_Label.Text = "Client State:";
            // 
            // ClientStateString_Label
            // 
            this.ClientStateString_Label.AutoSize = true;
            this.ClientStateString_Label.Location = new System.Drawing.Point(112, 15);
            this.ClientStateString_Label.Name = "ClientStateString_Label";
            this.ClientStateString_Label.Size = new System.Drawing.Size(58, 13);
            this.ClientStateString_Label.TabIndex = 1;
            this.ClientStateString_Label.Text = "Signed out";
            // 
            // Contact_ListBox
            // 
            this.Contact_ListBox.FormattingEnabled = true;
            this.Contact_ListBox.Location = new System.Drawing.Point(13, 63);
            this.Contact_ListBox.Name = "Contact_ListBox";
            this.Contact_ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Contact_ListBox.Size = new System.Drawing.Size(180, 82);
            this.Contact_ListBox.TabIndex = 2;
            // 
            // StartSharingResource_Button
            // 
            this.StartSharingResource_Button.Location = new System.Drawing.Point(12, 397);
            this.StartSharingResource_Button.Name = "StartSharingResource_Button";
            this.StartSharingResource_Button.Size = new System.Drawing.Size(153, 23);
            this.StartSharingResource_Button.TabIndex = 3;
            this.StartSharingResource_Button.Text = "4) Share Resource";
            this.StartSharingResource_Button.UseVisualStyleBackColor = true;
            this.StartSharingResource_Button.Click += new System.EventHandler(this.StartSharingResource_Button_Click);
            // 
            // Accept_Button
            // 
            this.Accept_Button.Enabled = false;
            this.Accept_Button.Location = new System.Drawing.Point(11, 28);
            this.Accept_Button.Name = "Accept_Button";
            this.Accept_Button.Size = new System.Drawing.Size(130, 23);
            this.Accept_Button.TabIndex = 4;
            this.Accept_Button.Text = "Accept";
            this.Accept_Button.UseVisualStyleBackColor = true;
            this.Accept_Button.Click += new System.EventHandler(this.Accept_Button_Click);
            // 
            // Decline_Button
            // 
            this.Decline_Button.Enabled = false;
            this.Decline_Button.Location = new System.Drawing.Point(11, 57);
            this.Decline_Button.Name = "Decline_Button";
            this.Decline_Button.Size = new System.Drawing.Size(130, 23);
            this.Decline_Button.TabIndex = 5;
            this.Decline_Button.Text = "Decline";
            this.Decline_Button.UseVisualStyleBackColor = true;
            this.Decline_Button.Click += new System.EventHandler(this.Decline_Button_Click);
            // 
            // Grant_Button
            // 
            this.Grant_Button.Enabled = false;
            this.Grant_Button.Location = new System.Drawing.Point(11, 86);
            this.Grant_Button.Name = "Grant_Button";
            this.Grant_Button.Size = new System.Drawing.Size(130, 23);
            this.Grant_Button.TabIndex = 6;
            this.Grant_Button.Text = "Grant";
            this.Grant_Button.UseVisualStyleBackColor = true;
            this.Grant_Button.Click += new System.EventHandler(this.Grant_Button_Click);
            // 
            // SharingAction_GroupBox
            // 
            this.SharingAction_GroupBox.Controls.Add(this.Disconnect_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Revoke_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Request_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Release_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Accept_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Grant_Button);
            this.SharingAction_GroupBox.Controls.Add(this.Decline_Button);
            this.SharingAction_GroupBox.Location = new System.Drawing.Point(235, 15);
            this.SharingAction_GroupBox.Name = "SharingAction_GroupBox";
            this.SharingAction_GroupBox.Size = new System.Drawing.Size(162, 232);
            this.SharingAction_GroupBox.TabIndex = 7;
            this.SharingAction_GroupBox.TabStop = false;
            this.SharingAction_GroupBox.Text = "6) Take a control action";
            // 
            // Disconnect_Button
            // 
            this.Disconnect_Button.Enabled = false;
            this.Disconnect_Button.Location = new System.Drawing.Point(11, 202);
            this.Disconnect_Button.Name = "Disconnect_Button";
            this.Disconnect_Button.Size = new System.Drawing.Size(130, 23);
            this.Disconnect_Button.TabIndex = 10;
            this.Disconnect_Button.Text = "Stop Sharing";
            this.Disconnect_Button.UseVisualStyleBackColor = true;
            this.Disconnect_Button.Click += new System.EventHandler(this.Disconnect_Button_Click);
            // 
            // Revoke_Button
            // 
            this.Revoke_Button.Enabled = false;
            this.Revoke_Button.Location = new System.Drawing.Point(11, 115);
            this.Revoke_Button.Name = "Revoke_Button";
            this.Revoke_Button.Size = new System.Drawing.Size(130, 23);
            this.Revoke_Button.TabIndex = 9;
            this.Revoke_Button.Text = "Revoke";
            this.Revoke_Button.UseVisualStyleBackColor = true;
            this.Revoke_Button.Click += new System.EventHandler(this.Revoke_Button_Click);
            // 
            // Request_Button
            // 
            this.Request_Button.Enabled = false;
            this.Request_Button.Location = new System.Drawing.Point(11, 144);
            this.Request_Button.Name = "Request_Button";
            this.Request_Button.Size = new System.Drawing.Size(130, 23);
            this.Request_Button.TabIndex = 8;
            this.Request_Button.Text = "Request";
            this.Request_Button.UseVisualStyleBackColor = true;
            this.Request_Button.Click += new System.EventHandler(this.Request_Button_Click);
            // 
            // Release_Button
            // 
            this.Release_Button.Enabled = false;
            this.Release_Button.Location = new System.Drawing.Point(11, 173);
            this.Release_Button.Name = "Release_Button";
            this.Release_Button.Size = new System.Drawing.Size(130, 23);
            this.Release_Button.TabIndex = 7;
            this.Release_Button.Text = "Release";
            this.Release_Button.UseVisualStyleBackColor = true;
            this.Release_Button.Click += new System.EventHandler(this.Release_Button_Click);
            // 
            // SharingParticipationState_Label
            // 
            this.SharingParticipationState_Label.AutoSize = true;
            this.SharingParticipationState_Label.Location = new System.Drawing.Point(240, 253);
            this.SharingParticipationState_Label.Name = "SharingParticipationState_Label";
            this.SharingParticipationState_Label.Size = new System.Drawing.Size(27, 13);
            this.SharingParticipationState_Label.TabIndex = 8;
            this.SharingParticipationState_Label.Text = "I am";
            // 
            // SharingParticipationStateString_Label
            // 
            this.SharingParticipationStateString_Label.AutoSize = true;
            this.SharingParticipationStateString_Label.Location = new System.Drawing.Point(264, 253);
            this.SharingParticipationStateString_Label.Name = "SharingParticipationStateString_Label";
            this.SharingParticipationStateString_Label.Size = new System.Drawing.Size(62, 13);
            this.SharingParticipationStateString_Label.TabIndex = 9;
            this.SharingParticipationStateString_Label.Text = " not sharing";
            // 
            // SharedResources_ListBox
            // 
            this.SharedResources_ListBox.FormattingEnabled = true;
            this.SharedResources_ListBox.HorizontalScrollbar = true;
            this.SharedResources_ListBox.Location = new System.Drawing.Point(12, 283);
            this.SharedResources_ListBox.Name = "SharedResources_ListBox";
            this.SharedResources_ListBox.Size = new System.Drawing.Size(181, 108);
            this.SharedResources_ListBox.TabIndex = 11;
            // 
            // ConversationActions_Group
            // 
            this.ConversationActions_Group.Controls.Add(this.EndConversation_Button);
            this.ConversationActions_Group.Controls.Add(this.Start_Button);
            this.ConversationActions_Group.Location = new System.Drawing.Point(12, 158);
            this.ConversationActions_Group.Name = "ConversationActions_Group";
            this.ConversationActions_Group.Size = new System.Drawing.Size(161, 89);
            this.ConversationActions_Group.TabIndex = 13;
            this.ConversationActions_Group.TabStop = false;
            this.ConversationActions_Group.Text = "2) Start a conversation";
            // 
            // EndConversation_Button
            // 
            this.EndConversation_Button.Enabled = false;
            this.EndConversation_Button.Location = new System.Drawing.Point(16, 50);
            this.EndConversation_Button.Name = "EndConversation_Button";
            this.EndConversation_Button.Size = new System.Drawing.Size(130, 23);
            this.EndConversation_Button.TabIndex = 0;
            this.EndConversation_Button.Text = "End";
            this.EndConversation_Button.UseVisualStyleBackColor = true;
            this.EndConversation_Button.Click += new System.EventHandler(this.EndConversation_Button_Click);
            // 
            // Start_Button
            // 
            this.Start_Button.Enabled = false;
            this.Start_Button.Location = new System.Drawing.Point(16, 21);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(130, 23);
            this.Start_Button.TabIndex = 16;
            this.Start_Button.Text = "Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // ResourceController_Label
            // 
            this.ResourceController_Label.AutoSize = true;
            this.ResourceController_Label.Location = new System.Drawing.Point(240, 300);
            this.ResourceController_Label.Name = "ResourceController_Label";
            this.ResourceController_Label.Size = new System.Drawing.Size(103, 13);
            this.ResourceController_Label.TabIndex = 14;
            this.ResourceController_Label.Text = "Resource Controller:";
            // 
            // ResourceControllerName_Label
            // 
            this.ResourceControllerName_Label.AutoSize = true;
            this.ResourceControllerName_Label.Location = new System.Drawing.Point(240, 322);
            this.ResourceControllerName_Label.Name = "ResourceControllerName_Label";
            this.ResourceControllerName_Label.Size = new System.Drawing.Size(0, 13);
            this.ResourceControllerName_Label.TabIndex = 15;
            // 
            // ShareableResources_Label
            // 
            this.ShareableResources_Label.AutoSize = true;
            this.ShareableResources_Label.Location = new System.Drawing.Point(13, 267);
            this.ShareableResources_Label.Name = "ShareableResources_Label";
            this.ShareableResources_Label.Size = new System.Drawing.Size(93, 13);
            this.ShareableResources_Label.TabIndex = 17;
            this.ShareableResources_Label.Text = "3) Pick a resource";
            // 
            // ContactList_Label
            // 
            this.ContactList_Label.AutoSize = true;
            this.ContactList_Label.Location = new System.Drawing.Point(12, 44);
            this.ContactList_Label.Name = "ContactList_Label";
            this.ContactList_Label.Size = new System.Drawing.Size(99, 13);
            this.ContactList_Label.TabIndex = 20;
            this.ContactList_Label.Text = "1) Choose contacts";
            // 
            // SharedResource_Label
            // 
            this.SharedResource_Label.AutoSize = true;
            this.SharedResource_Label.Location = new System.Drawing.Point(243, 350);
            this.SharedResource_Label.Name = "SharedResource_Label";
            this.SharedResource_Label.Size = new System.Drawing.Size(33, 13);
            this.SharedResource_Label.TabIndex = 21;
            this.SharedResource_Label.Text = "None";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RejectInvitation_Button);
            this.groupBox1.Controls.Add(this.AcceptInvitation_Button);
            this.groupBox1.Location = new System.Drawing.Point(235, 381);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 96);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sharing Invitiation Action";
            // 
            // RejectInvitation_Button
            // 
            this.RejectInvitation_Button.Enabled = false;
            this.RejectInvitation_Button.Location = new System.Drawing.Point(11, 48);
            this.RejectInvitation_Button.Name = "RejectInvitation_Button";
            this.RejectInvitation_Button.Size = new System.Drawing.Size(130, 23);
            this.RejectInvitation_Button.TabIndex = 1;
            this.RejectInvitation_Button.Text = "Reject";
            this.RejectInvitation_Button.UseVisualStyleBackColor = true;
            this.RejectInvitation_Button.Click += new System.EventHandler(this.RejectInvitation_Button_Click);
            // 
            // AcceptInvitation_Button
            // 
            this.AcceptInvitation_Button.Enabled = false;
            this.AcceptInvitation_Button.Location = new System.Drawing.Point(11, 19);
            this.AcceptInvitation_Button.Name = "AcceptInvitation_Button";
            this.AcceptInvitation_Button.Size = new System.Drawing.Size(130, 23);
            this.AcceptInvitation_Button.TabIndex = 0;
            this.AcceptInvitation_Button.Text = "Accept";
            this.AcceptInvitation_Button.UseVisualStyleBackColor = true;
            this.AcceptInvitation_Button.Click += new System.EventHandler(this.AcceptInvitation_Button_Click);
            // 
            // RefreshResource_Button
            // 
            this.RefreshResource_Button.Location = new System.Drawing.Point(12, 445);
            this.RefreshResource_Button.Name = "RefreshResource_Button";
            this.RefreshResource_Button.Size = new System.Drawing.Size(153, 23);
            this.RefreshResource_Button.TabIndex = 23;
            this.RefreshResource_Button.Text = "Refresh resources";
            this.RefreshResource_Button.UseVisualStyleBackColor = true;
            this.RefreshResource_Button.Click += new System.EventHandler(this.RefreshResource_Button_Click);
            // 
            // ShareResources_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 489);
            this.Controls.Add(this.RefreshResource_Button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SharedResource_Label);
            this.Controls.Add(this.ContactList_Label);
            this.Controls.Add(this.ShareableResources_Label);
            this.Controls.Add(this.ResourceControllerName_Label);
            this.Controls.Add(this.ResourceController_Label);
            this.Controls.Add(this.ConversationActions_Group);
            this.Controls.Add(this.SharedResources_ListBox);
            this.Controls.Add(this.SharingParticipationStateString_Label);
            this.Controls.Add(this.SharingParticipationState_Label);
            this.Controls.Add(this.SharingAction_GroupBox);
            this.Controls.Add(this.StartSharingResource_Button);
            this.Controls.Add(this.Contact_ListBox);
            this.Controls.Add(this.ClientStateString_Label);
            this.Controls.Add(this.ClientState_Label);
            this.Name = "ShareResources_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Share Resources";
            this.Load += new System.EventHandler(this.ShareResources_Form_Load);
            this.SharingAction_GroupBox.ResumeLayout(false);
            this.ConversationActions_Group.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ClientState_Label;
        private System.Windows.Forms.Label ClientStateString_Label;
        private System.Windows.Forms.ListBox Contact_ListBox;
        private System.Windows.Forms.Button StartSharingResource_Button;
        private System.Windows.Forms.Button Accept_Button;
        private System.Windows.Forms.Button Decline_Button;
        private System.Windows.Forms.Button Grant_Button;
        private System.Windows.Forms.GroupBox SharingAction_GroupBox;
        private System.Windows.Forms.Button Release_Button;
        private System.Windows.Forms.Button Revoke_Button;
        private System.Windows.Forms.Button Request_Button;
        private System.Windows.Forms.Label SharingParticipationState_Label;
        private System.Windows.Forms.Label SharingParticipationStateString_Label;
        private System.Windows.Forms.ListBox SharedResources_ListBox;
        private System.Windows.Forms.GroupBox ConversationActions_Group;
        private System.Windows.Forms.Button EndConversation_Button;
        private System.Windows.Forms.Button Disconnect_Button;
        private System.Windows.Forms.Label ResourceController_Label;
        private System.Windows.Forms.Label ResourceControllerName_Label;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Label ShareableResources_Label;
        private System.Windows.Forms.Label ContactList_Label;
        private System.Windows.Forms.Label SharedResource_Label;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button RejectInvitation_Button;
        private System.Windows.Forms.Button AcceptInvitation_Button;
        private System.Windows.Forms.Button RefreshResource_Button;
    }
}

