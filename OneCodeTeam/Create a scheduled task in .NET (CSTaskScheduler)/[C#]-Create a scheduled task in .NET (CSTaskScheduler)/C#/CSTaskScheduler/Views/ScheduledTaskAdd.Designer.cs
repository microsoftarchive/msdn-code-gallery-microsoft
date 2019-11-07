// ----------------------------------------------------------------------
// <copyright file="ScheduledTaskAdd.Designer.cs" company="Microsoft">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------

namespace CSTaskScheduler.Views
{
    /// <summary>
    /// Code behind for Add form
    /// </summary>
    public partial class ScheduledTaskAdd
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
            this.components = new System.ComponentModel.Container();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.CancelAction = new System.Windows.Forms.Button();
            this.CreateAction = new System.Windows.Forms.Button();
            this.DescriptionData = new System.Windows.Forms.TextBox();
            this.DescriptionText = new System.Windows.Forms.Label();
            this.ScheduledTaskMetadataTabControl = new System.Windows.Forms.TabControl();
            this.GeneralTabPage = new System.Windows.Forms.TabPage();
            this.IsHiddenOption = new System.Windows.Forms.CheckBox();
            this.SecurityOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.RunUserLoggedOnOption = new System.Windows.Forms.RadioButton();
            this.RunUserLoggedOnOrNotOption = new System.Windows.Forms.RadioButton();
            this.SecurityUserNameText = new System.Windows.Forms.Label();
            this.SecurityText = new System.Windows.Forms.Label();
            this.TriggerTabPage = new System.Windows.Forms.TabPage();
            this.SettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.ExpireOption = new System.Windows.Forms.CheckBox();
            this.EnableOption = new System.Windows.Forms.CheckBox();
            this.StartDateTimeText = new System.Windows.Forms.Label();
            this.EndDateTimeData = new System.Windows.Forms.DateTimePicker();
            this.StartDateTimeData = new System.Windows.Forms.DateTimePicker();
            this.SettingsText = new System.Windows.Forms.Label();
            this.ActionTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ArgumentsData = new System.Windows.Forms.TextBox();
            this.ArgumentsText = new System.Windows.Forms.Label();
            this.ChooseFile = new System.Windows.Forms.Button();
            this.SelectedFileData = new System.Windows.Forms.TextBox();
            this.ProgramScriptText = new System.Windows.Forms.Label();
            this.ConditionTabPage = new System.Windows.Forms.TabPage();
            this.MinutesData = new System.Windows.Forms.NumericUpDown();
            this.MinutesText = new System.Windows.Forms.Label();
            this.IdleOption = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ConditionDescriptionText = new System.Windows.Forms.Label();
            this.SettingsTabPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StopOption = new System.Windows.Forms.CheckBox();
            this.StopTaskAfterMinutesData = new System.Windows.Forms.NumericUpDown();
            this.LocationData = new System.Windows.Forms.Label();
            this.LocationText = new System.Windows.Forms.Label();
            this.NameData = new System.Windows.Forms.TextBox();
            this.NameText = new System.Windows.Forms.Label();
            this.CreateTaskText = new System.Windows.Forms.Label();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox.SuspendLayout();
            this.ScheduledTaskMetadataTabControl.SuspendLayout();
            this.GeneralTabPage.SuspendLayout();
            this.SecurityOptionsGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.TriggerTabPage.SuspendLayout();
            this.SettingsGroupBox.SuspendLayout();
            this.ActionTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ConditionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesData)).BeginInit();
            this.SettingsTabPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StopTaskAfterMinutesData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.CancelAction);
            this.groupBox.Controls.Add(this.CreateAction);
            this.groupBox.Controls.Add(this.DescriptionData);
            this.groupBox.Controls.Add(this.DescriptionText);
            this.groupBox.Controls.Add(this.ScheduledTaskMetadataTabControl);
            this.groupBox.Controls.Add(this.LocationData);
            this.groupBox.Controls.Add(this.LocationText);
            this.groupBox.Controls.Add(this.NameData);
            this.groupBox.Controls.Add(this.NameText);
            this.groupBox.Controls.Add(this.CreateTaskText);
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(801, 624);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            // 
            // CancelAction
            // 
            this.CancelAction.Location = new System.Drawing.Point(641, 560);
            this.CancelAction.Name = "CancelAction";
            this.CancelAction.Size = new System.Drawing.Size(75, 23);
            this.CancelAction.TabIndex = 9;
            this.CancelAction.Text = "Cancel";
            this.CancelAction.UseVisualStyleBackColor = true;
            // 
            // CreateAction
            // 
            this.CreateAction.Location = new System.Drawing.Point(538, 560);
            this.CreateAction.Name = "CreateAction";
            this.CreateAction.Size = new System.Drawing.Size(75, 23);
            this.CreateAction.TabIndex = 8;
            this.CreateAction.Text = "Create";
            this.CreateAction.UseVisualStyleBackColor = true;
            // 
            // DescriptionData
            // 
            this.DescriptionData.Location = new System.Drawing.Point(107, 144);
            this.DescriptionData.MaxLength = 250;
            this.DescriptionData.Multiline = true;
            this.DescriptionData.Name = "DescriptionData";
            this.DescriptionData.Size = new System.Drawing.Size(460, 49);
            this.DescriptionData.TabIndex = 7;
            // 
            // DescriptionText
            // 
            this.DescriptionText.AutoSize = true;
            this.DescriptionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionText.Location = new System.Drawing.Point(35, 147);
            this.DescriptionText.Name = "DescriptionText";
            this.DescriptionText.Size = new System.Drawing.Size(63, 13);
            this.DescriptionText.TabIndex = 6;
            this.DescriptionText.Text = "Description:";
            // 
            // ScheduledTaskMetadataTabControl
            // 
            this.ScheduledTaskMetadataTabControl.Controls.Add(this.GeneralTabPage);
            this.ScheduledTaskMetadataTabControl.Controls.Add(this.TriggerTabPage);
            this.ScheduledTaskMetadataTabControl.Controls.Add(this.ActionTabPage);
            this.ScheduledTaskMetadataTabControl.Controls.Add(this.ConditionTabPage);
            this.ScheduledTaskMetadataTabControl.Controls.Add(this.SettingsTabPage);
            this.ScheduledTaskMetadataTabControl.Location = new System.Drawing.Point(29, 222);
            this.ScheduledTaskMetadataTabControl.Name = "ScheduledTaskMetadataTabControl";
            this.ScheduledTaskMetadataTabControl.SelectedIndex = 0;
            this.ScheduledTaskMetadataTabControl.Size = new System.Drawing.Size(729, 316);
            this.ScheduledTaskMetadataTabControl.TabIndex = 5;
            // 
            // GeneralTabPage
            // 
            this.GeneralTabPage.Controls.Add(this.IsHiddenOption);
            this.GeneralTabPage.Controls.Add(this.SecurityOptionsGroupBox);
            this.GeneralTabPage.Location = new System.Drawing.Point(4, 22);
            this.GeneralTabPage.Name = "GeneralTabPage";
            this.GeneralTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralTabPage.Size = new System.Drawing.Size(721, 290);
            this.GeneralTabPage.TabIndex = 0;
            this.GeneralTabPage.Text = "General";
            this.GeneralTabPage.UseVisualStyleBackColor = true;
            // 
            // IsHiddenOption
            // 
            this.IsHiddenOption.AutoSize = true;
            this.IsHiddenOption.Location = new System.Drawing.Point(32, 204);
            this.IsHiddenOption.Name = "IsHiddenOption";
            this.IsHiddenOption.Size = new System.Drawing.Size(60, 17);
            this.IsHiddenOption.TabIndex = 1;
            this.IsHiddenOption.Text = "Hidden";
            this.IsHiddenOption.UseVisualStyleBackColor = true;
            // 
            // SecurityOptionsGroupBox
            // 
            this.SecurityOptionsGroupBox.Controls.Add(this.groupBox3);
            this.SecurityOptionsGroupBox.Controls.Add(this.SecurityUserNameText);
            this.SecurityOptionsGroupBox.Controls.Add(this.SecurityText);
            this.SecurityOptionsGroupBox.Location = new System.Drawing.Point(6, 23);
            this.SecurityOptionsGroupBox.Name = "SecurityOptionsGroupBox";
            this.SecurityOptionsGroupBox.Size = new System.Drawing.Size(709, 159);
            this.SecurityOptionsGroupBox.TabIndex = 0;
            this.SecurityOptionsGroupBox.TabStop = false;
            this.SecurityOptionsGroupBox.Text = "Security Options";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.RunUserLoggedOnOption);
            this.groupBox3.Controls.Add(this.RunUserLoggedOnOrNotOption);
            this.groupBox3.Location = new System.Drawing.Point(15, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(269, 76);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            // 
            // RunUserLoggedOnOption
            // 
            this.RunUserLoggedOnOption.AutoSize = true;
            this.RunUserLoggedOnOption.Location = new System.Drawing.Point(16, 19);
            this.RunUserLoggedOnOption.Name = "RunUserLoggedOnOption";
            this.RunUserLoggedOnOption.Size = new System.Drawing.Size(157, 17);
            this.RunUserLoggedOnOption.TabIndex = 2;
            this.RunUserLoggedOnOption.TabStop = true;
            this.RunUserLoggedOnOption.Text = "Run when user is logged on";
            this.RunUserLoggedOnOption.UseVisualStyleBackColor = true;
            // 
            // RunUserLoggedOnOrNotOption
            // 
            this.RunUserLoggedOnOrNotOption.AutoSize = true;
            this.RunUserLoggedOnOrNotOption.Location = new System.Drawing.Point(16, 51);
            this.RunUserLoggedOnOrNotOption.Name = "RunUserLoggedOnOrNotOption";
            this.RunUserLoggedOnOrNotOption.Size = new System.Drawing.Size(187, 17);
            this.RunUserLoggedOnOrNotOption.TabIndex = 3;
            this.RunUserLoggedOnOrNotOption.TabStop = true;
            this.RunUserLoggedOnOrNotOption.Text = "Run when user is logged on or not";
            this.RunUserLoggedOnOrNotOption.UseVisualStyleBackColor = true;
            // 
            // SecurityUserNameText
            // 
            this.SecurityUserNameText.AutoSize = true;
            this.SecurityUserNameText.BackColor = System.Drawing.Color.LightGray;
            this.SecurityUserNameText.Location = new System.Drawing.Point(23, 56);
            this.SecurityUserNameText.Name = "SecurityUserNameText";
            this.SecurityUserNameText.Size = new System.Drawing.Size(143, 13);
            this.SecurityUserNameText.TabIndex = 1;
            this.SecurityUserNameText.Text = "username along with Domain";
            // 
            // SecurityText
            // 
            this.SecurityText.AutoSize = true;
            this.SecurityText.Location = new System.Drawing.Point(12, 30);
            this.SecurityText.Name = "SecurityText";
            this.SecurityText.Size = new System.Drawing.Size(268, 13);
            this.SecurityText.TabIndex = 0;
            this.SecurityText.Text = "When running the task, use the following user account:";
            // 
            // TriggerTabPage
            // 
            this.TriggerTabPage.Controls.Add(this.SettingsGroupBox);
            this.TriggerTabPage.Location = new System.Drawing.Point(4, 22);
            this.TriggerTabPage.Name = "TriggerTabPage";
            this.TriggerTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.TriggerTabPage.Size = new System.Drawing.Size(721, 290);
            this.TriggerTabPage.TabIndex = 1;
            this.TriggerTabPage.Text = "Trigger";
            this.TriggerTabPage.UseVisualStyleBackColor = true;
            // 
            // SettingsGroupBox
            // 
            this.SettingsGroupBox.Controls.Add(this.ExpireOption);
            this.SettingsGroupBox.Controls.Add(this.EnableOption);
            this.SettingsGroupBox.Controls.Add(this.StartDateTimeText);
            this.SettingsGroupBox.Controls.Add(this.EndDateTimeData);
            this.SettingsGroupBox.Controls.Add(this.StartDateTimeData);
            this.SettingsGroupBox.Controls.Add(this.SettingsText);
            this.SettingsGroupBox.Location = new System.Drawing.Point(10, 29);
            this.SettingsGroupBox.Name = "SettingsGroupBox";
            this.SettingsGroupBox.Size = new System.Drawing.Size(685, 242);
            this.SettingsGroupBox.TabIndex = 0;
            this.SettingsGroupBox.TabStop = false;
            this.SettingsGroupBox.Text = "Settings";
            // 
            // ExpireOption
            // 
            this.ExpireOption.AutoSize = true;
            this.ExpireOption.Location = new System.Drawing.Point(24, 106);
            this.ExpireOption.Name = "ExpireOption";
            this.ExpireOption.Size = new System.Drawing.Size(100, 17);
            this.ExpireOption.TabIndex = 1;
            this.ExpireOption.Text = "End Date Time:";
            this.ExpireOption.UseVisualStyleBackColor = true;
            // 
            // EnableOption
            // 
            this.EnableOption.AutoSize = true;
            this.EnableOption.Location = new System.Drawing.Point(41, 163);
            this.EnableOption.Name = "EnableOption";
            this.EnableOption.Size = new System.Drawing.Size(65, 17);
            this.EnableOption.TabIndex = 6;
            this.EnableOption.Text = "Enabled";
            this.EnableOption.UseVisualStyleBackColor = true;
            // 
            // StartDateTimeText
            // 
            this.StartDateTimeText.AutoSize = true;
            this.StartDateTimeText.Location = new System.Drawing.Point(38, 66);
            this.StartDateTimeText.Name = "StartDateTimeText";
            this.StartDateTimeText.Size = new System.Drawing.Size(81, 13);
            this.StartDateTimeText.TabIndex = 3;
            this.StartDateTimeText.Text = "Start DateTime:";
            // 
            // EndDateTimeData
            // 
            this.EndDateTimeData.CustomFormat = "dd-MM-yyyy h:mm:ss";
            this.EndDateTimeData.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.EndDateTimeData.Location = new System.Drawing.Point(127, 106);
            this.EndDateTimeData.Name = "EndDateTimeData";
            this.EndDateTimeData.Size = new System.Drawing.Size(173, 20);
            this.EndDateTimeData.TabIndex = 2;
            // 
            // StartDateTimeData
            // 
            this.StartDateTimeData.CustomFormat = "dd-MM-yyyy h:mm:ss";
            this.StartDateTimeData.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.StartDateTimeData.Location = new System.Drawing.Point(127, 66);
            this.StartDateTimeData.Name = "StartDateTimeData";
            this.StartDateTimeData.Size = new System.Drawing.Size(173, 20);
            this.StartDateTimeData.TabIndex = 1;
            // 
            // SettingsText
            // 
            this.SettingsText.AutoSize = true;
            this.SettingsText.Location = new System.Drawing.Point(21, 36);
            this.SettingsText.Name = "SettingsText";
            this.SettingsText.Size = new System.Drawing.Size(220, 13);
            this.SettingsText.TabIndex = 0;
            this.SettingsText.Text = "Run the task Daily with the following settings:";
            // 
            // ActionTabPage
            // 
            this.ActionTabPage.Controls.Add(this.groupBox1);
            this.ActionTabPage.Location = new System.Drawing.Point(4, 22);
            this.ActionTabPage.Name = "ActionTabPage";
            this.ActionTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ActionTabPage.Size = new System.Drawing.Size(721, 290);
            this.ActionTabPage.TabIndex = 2;
            this.ActionTabPage.Text = "Action";
            this.ActionTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ArgumentsData);
            this.groupBox1.Controls.Add(this.ArgumentsText);
            this.groupBox1.Controls.Add(this.ChooseFile);
            this.groupBox1.Controls.Add(this.SelectedFileData);
            this.groupBox1.Controls.Add(this.ProgramScriptText);
            this.groupBox1.Location = new System.Drawing.Point(6, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(692, 183);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Specify a program to start";
            // 
            // ArgumentsData
            // 
            this.ArgumentsData.Location = new System.Drawing.Point(172, 104);
            this.ArgumentsData.Name = "ArgumentsData";
            this.ArgumentsData.Size = new System.Drawing.Size(161, 20);
            this.ArgumentsData.TabIndex = 4;
            // 
            // ArgumentsText
            // 
            this.ArgumentsText.AutoSize = true;
            this.ArgumentsText.Location = new System.Drawing.Point(24, 104);
            this.ArgumentsText.Name = "ArgumentsText";
            this.ArgumentsText.Size = new System.Drawing.Size(128, 13);
            this.ArgumentsText.TabIndex = 3;
            this.ArgumentsText.Text = "Add Arguments (optional):";
            // 
            // ChooseFile
            // 
            this.ChooseFile.Location = new System.Drawing.Point(572, 59);
            this.ChooseFile.Name = "ChooseFile";
            this.ChooseFile.Size = new System.Drawing.Size(75, 23);
            this.ChooseFile.TabIndex = 2;
            this.ChooseFile.Text = "Browse";
            this.ChooseFile.UseVisualStyleBackColor = true;
            // 
            // SelectedFileData
            // 
            this.SelectedFileData.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SelectedFileData.Location = new System.Drawing.Point(27, 62);
            this.SelectedFileData.Name = "SelectedFileData";
            this.SelectedFileData.Size = new System.Drawing.Size(513, 20);
            this.SelectedFileData.TabIndex = 1;
            // 
            // ProgramScriptText
            // 
            this.ProgramScriptText.AutoSize = true;
            this.ProgramScriptText.Location = new System.Drawing.Point(24, 39);
            this.ProgramScriptText.Name = "ProgramScriptText";
            this.ProgramScriptText.Size = new System.Drawing.Size(81, 13);
            this.ProgramScriptText.TabIndex = 0;
            this.ProgramScriptText.Text = "Program\\Script:";
            // 
            // ConditionTabPage
            // 
            this.ConditionTabPage.Controls.Add(this.MinutesData);
            this.ConditionTabPage.Controls.Add(this.MinutesText);
            this.ConditionTabPage.Controls.Add(this.IdleOption);
            this.ConditionTabPage.Controls.Add(this.label1);
            this.ConditionTabPage.Controls.Add(this.ConditionDescriptionText);
            this.ConditionTabPage.Location = new System.Drawing.Point(4, 22);
            this.ConditionTabPage.Name = "ConditionTabPage";
            this.ConditionTabPage.Size = new System.Drawing.Size(721, 290);
            this.ConditionTabPage.TabIndex = 3;
            this.ConditionTabPage.Text = "Condition";
            this.ConditionTabPage.UseVisualStyleBackColor = true;
            // 
            // MinutesData
            // 
            this.MinutesData.Enabled = false;
            this.MinutesData.Location = new System.Drawing.Point(252, 104);
            this.MinutesData.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinutesData.Name = "MinutesData";
            this.MinutesData.Size = new System.Drawing.Size(43, 20);
            this.MinutesData.TabIndex = 5;
            this.MinutesData.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MinutesText
            // 
            this.MinutesText.AutoSize = true;
            this.MinutesText.Location = new System.Drawing.Point(301, 108);
            this.MinutesText.Name = "MinutesText";
            this.MinutesText.Size = new System.Drawing.Size(43, 13);
            this.MinutesText.TabIndex = 4;
            this.MinutesText.Text = "minutes";
            // 
            // IdleOption
            // 
            this.IdleOption.AutoSize = true;
            this.IdleOption.Location = new System.Drawing.Point(21, 107);
            this.IdleOption.Name = "IdleOption";
            this.IdleOption.Size = new System.Drawing.Size(213, 17);
            this.IdleOption.TabIndex = 2;
            this.IdleOption.Text = "Start the task only if computer is idle for:";
            this.IdleOption.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "if any condition specified here is not true.";
            // 
            // ConditionDescriptionText
            // 
            this.ConditionDescriptionText.AutoSize = true;
            this.ConditionDescriptionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConditionDescriptionText.Location = new System.Drawing.Point(18, 38);
            this.ConditionDescriptionText.Name = "ConditionDescriptionText";
            this.ConditionDescriptionText.Size = new System.Drawing.Size(620, 16);
            this.ConditionDescriptionText.TabIndex = 0;
            this.ConditionDescriptionText.Text = "Specify the conditions that, along with the trigger, determine whether task shoul" +
    "d run. The task will not run ";
            this.ConditionDescriptionText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SettingsTabPage
            // 
            this.SettingsTabPage.Controls.Add(this.groupBox2);
            this.SettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsTabPage.Name = "SettingsTabPage";
            this.SettingsTabPage.Size = new System.Drawing.Size(721, 290);
            this.SettingsTabPage.TabIndex = 4;
            this.SettingsTabPage.Text = "Settings";
            this.SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.StopOption);
            this.groupBox2.Controls.Add(this.StopTaskAfterMinutesData);
            this.groupBox2.Location = new System.Drawing.Point(5, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(703, 221);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Specify additional settings that affect the behavior of this task";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "minutes";
            // 
            // StopOption
            // 
            this.StopOption.AutoSize = true;
            this.StopOption.Location = new System.Drawing.Point(23, 53);
            this.StopOption.Name = "StopOption";
            this.StopOption.Size = new System.Drawing.Size(187, 17);
            this.StopOption.TabIndex = 2;
            this.StopOption.Text = "Stop the task if it runs longer than:";
            this.StopOption.UseVisualStyleBackColor = true;
            // 
            // StopTaskAfterMinutesData
            // 
            this.StopTaskAfterMinutesData.Location = new System.Drawing.Point(230, 52);
            this.StopTaskAfterMinutesData.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.StopTaskAfterMinutesData.Name = "StopTaskAfterMinutesData";
            this.StopTaskAfterMinutesData.Size = new System.Drawing.Size(53, 20);
            this.StopTaskAfterMinutesData.TabIndex = 1;
            this.StopTaskAfterMinutesData.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // LocationData
            // 
            this.LocationData.AutoSize = true;
            this.LocationData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationData.Location = new System.Drawing.Point(104, 112);
            this.LocationData.Name = "LocationData";
            this.LocationData.Size = new System.Drawing.Size(12, 13);
            this.LocationData.TabIndex = 4;
            this.LocationData.Text = "\\";
            // 
            // LocationText
            // 
            this.LocationText.AutoSize = true;
            this.LocationText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationText.Location = new System.Drawing.Point(35, 112);
            this.LocationText.Name = "LocationText";
            this.LocationText.Size = new System.Drawing.Size(51, 13);
            this.LocationText.TabIndex = 3;
            this.LocationText.Text = "Location:";
            // 
            // NameData
            // 
            this.NameData.Location = new System.Drawing.Point(107, 70);
            this.NameData.MaxLength = 50;
            this.NameData.Name = "NameData";
            this.NameData.Size = new System.Drawing.Size(100, 20);
            this.NameData.TabIndex = 2;
            // 
            // NameText
            // 
            this.NameText.AutoSize = true;
            this.NameText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameText.Location = new System.Drawing.Point(35, 73);
            this.NameText.Name = "NameText";
            this.NameText.Size = new System.Drawing.Size(38, 13);
            this.NameText.TabIndex = 1;
            this.NameText.Text = "Name:";
            // 
            // CreateTaskText
            // 
            this.CreateTaskText.AutoSize = true;
            this.CreateTaskText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateTaskText.Location = new System.Drawing.Point(35, 44);
            this.CreateTaskText.Name = "CreateTaskText";
            this.CreateTaskText.Size = new System.Drawing.Size(430, 13);
            this.CreateTaskText.TabIndex = 0;
            this.CreateTaskText.Text = "Create or Update a Scheduled Task by providing the following information:";
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.ContainerControl = this;
            // 
            // ScheduledTaskAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 626);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.Name = "ScheduledTaskAdd";
            this.Text = "ScheduledTaskAdd";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ScheduledTaskMetadataTabControl.ResumeLayout(false);
            this.GeneralTabPage.ResumeLayout(false);
            this.GeneralTabPage.PerformLayout();
            this.SecurityOptionsGroupBox.ResumeLayout(false);
            this.SecurityOptionsGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.TriggerTabPage.ResumeLayout(false);
            this.SettingsGroupBox.ResumeLayout(false);
            this.SettingsGroupBox.PerformLayout();
            this.ActionTabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ConditionTabPage.ResumeLayout(false);
            this.ConditionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesData)).EndInit();
            this.SettingsTabPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StopTaskAfterMinutesData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.TextBox NameData;
        private System.Windows.Forms.Label NameText;
        private System.Windows.Forms.Label CreateTaskText;
        private System.Windows.Forms.Label LocationText;
        private System.Windows.Forms.Label LocationData;
        private System.Windows.Forms.TabControl ScheduledTaskMetadataTabControl;
        private System.Windows.Forms.TabPage GeneralTabPage;
        private System.Windows.Forms.TabPage TriggerTabPage;
        private System.Windows.Forms.TextBox DescriptionData;
        private System.Windows.Forms.Label DescriptionText;
        private System.Windows.Forms.TabPage ActionTabPage;
        private System.Windows.Forms.TabPage ConditionTabPage;
        private System.Windows.Forms.GroupBox SecurityOptionsGroupBox;
        private System.Windows.Forms.Label SecurityUserNameText;
        private System.Windows.Forms.Label SecurityText;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.RadioButton RunUserLoggedOnOrNotOption;
        private System.Windows.Forms.RadioButton RunUserLoggedOnOption;
        private System.Windows.Forms.CheckBox IsHiddenOption;
        private System.Windows.Forms.GroupBox SettingsGroupBox;
        private System.Windows.Forms.Label StartDateTimeText;
        private System.Windows.Forms.DateTimePicker EndDateTimeData;
        private System.Windows.Forms.DateTimePicker StartDateTimeData;
        private System.Windows.Forms.Label SettingsText;
        private System.Windows.Forms.CheckBox EnableOption;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox ArgumentsData;
        private System.Windows.Forms.Label ArgumentsText;
        private System.Windows.Forms.Button ChooseFile;
        private System.Windows.Forms.TextBox SelectedFileData;
        private System.Windows.Forms.Label ProgramScriptText;
        private System.Windows.Forms.Label ConditionDescriptionText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox IdleOption;
        private System.Windows.Forms.Label MinutesText;
        private System.Windows.Forms.NumericUpDown MinutesData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox StopOption;
        private System.Windows.Forms.NumericUpDown StopTaskAfterMinutesData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button CancelAction;
        private System.Windows.Forms.Button CreateAction;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.ErrorProvider ErrorProvider;
        private System.Windows.Forms.CheckBox ExpireOption;
    }
}