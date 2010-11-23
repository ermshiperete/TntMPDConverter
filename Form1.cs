namespace TntMPDConverter
{
    using System;
    using System.ComponentModel;
	using System.IO;
    using System.Windows.Forms;
    using Properties;

    public class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            if ((edtSourceFile.Text.Length > 0) && (edtTargetPath.Text.Length > 0))
            {
                Settings.Default.SourceFile = edtSourceFile.Text;
                Settings.Default.TargetPath = edtTargetPath.Text;
                var statement1 = new ConvertStatement();
                statement1.DoConversion();
                MessageBox.Show("Konvertierung abgeschlossen");
                Settings.Default.Save();
            }
        }

        private void OnSourceClick(object sender, EventArgs e)
        {
            openFileDialog.FileName = edtSourceFile.Text;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                edtSourceFile.Text = openFileDialog.FileName;
                if (edtTargetPath.Text.Length == 0)
                {
                    edtTargetPath.Text = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }
        }

        private void OnTargetClick(object sender, EventArgs e)
        {
        	folderBrowserDialog.SelectedPath = edtTargetPath.Text.Length > 0 ? 
				edtTargetPath.Text : Path.GetDirectoryName(edtSourceFile.Text);

        	if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                edtTargetPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

    	protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            edtSourceFile.Text = Settings.Default.SourceFile;
            edtTargetPath.Text = Settings.Default.TargetPath;
        }

        private void InitializeComponent()
        {
			label1 = new Label();
			edtSourceFile = new TextBox();
			btnSource = new Button();
			label2 = new Label();
			edtTargetPath = new TextBox();
			btnTarget = new Button();
			btnOk = new Button();
			btnCancel = new Button();
			openFileDialog = new OpenFileDialog();
			folderBrowserDialog = new FolderBrowserDialog();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 13);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(97, 13);
			label1.TabIndex = 0;
			label1.Text = "Projektabrechnung";
			// 
			// edtSourceFile
			// 
			edtSourceFile.Location = new System.Drawing.Point(16, 30);
			edtSourceFile.Name = "edtSourceFile";
			edtSourceFile.Size = new System.Drawing.Size(230, 20);
			edtSourceFile.TabIndex = 1;
			// 
			// btnSource
			// 
			btnSource.Location = new System.Drawing.Point(252, 27);
			btnSource.Name = "btnSource";
			btnSource.Size = new System.Drawing.Size(28, 23);
			btnSource.TabIndex = 2;
			btnSource.Text = "...";
			btnSource.UseVisualStyleBackColor = true;
			btnSource.Click += OnSourceClick;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(13, 57);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(77, 13);
			label2.TabIndex = 3;
			label2.Text = "Zielverzeichnis";
			// 
			// edtTargetPath
			// 
			edtTargetPath.Location = new System.Drawing.Point(16, 74);
			edtTargetPath.Name = "edtTargetPath";
			edtTargetPath.Size = new System.Drawing.Size(230, 20);
			edtTargetPath.TabIndex = 4;
			// 
			// btnTarget
			// 
			btnTarget.Location = new System.Drawing.Point(252, 71);
			btnTarget.Name = "btnTarget";
			btnTarget.Size = new System.Drawing.Size(28, 23);
			btnTarget.TabIndex = 5;
			btnTarget.Text = "...";
			btnTarget.UseVisualStyleBackColor = true;
			btnTarget.Click += OnTargetClick;
			// 
			// btnOk
			// 
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new System.Drawing.Point(56, 109);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(75, 23);
			btnOk.TabIndex = 6;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			btnOk.Click += OnOkClick;
			// 
			// btnCancel
			// 
			btnCancel.Location = new System.Drawing.Point(146, 109);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(75, 23);
			btnCancel.TabIndex = 7;
			btnCancel.Text = "Abbrechen";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// openFileDialog
			// 
			openFileDialog.FileName = "Projektabrechnung.txt";
			openFileDialog.Title = "Projektabrechnung auswählen";
			// 
			// folderBrowserDialog
			// 
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Personal;
			// 
			// Form1
			// 
			AcceptButton = btnOk;
			CancelButton = btnCancel;
			ClientSize = new System.Drawing.Size(292, 148);
			Controls.Add(btnCancel);
			Controls.Add(btnOk);
			Controls.Add(btnTarget);
			Controls.Add(edtTargetPath);
			Controls.Add(btnSource);
			Controls.Add(label2);
			Controls.Add(edtSourceFile);
			Controls.Add(label1);
			Name = "Form1";
			Text = "Projektabrechnung konvertieren";
			Shown += Form1_Shown;
			ResumeLayout(false);
			PerformLayout();

        }


        private Button btnCancel;
        private Button btnOk;
        private Button btnSource;
        private Button btnTarget;
        private Component components = new Component();
        private TextBox edtSourceFile;
        private TextBox edtTargetPath;
        private FolderBrowserDialog folderBrowserDialog;
        private Label label1;
        private Label label2;
        private OpenFileDialog openFileDialog;
    }
}

