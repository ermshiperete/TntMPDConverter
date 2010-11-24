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
				MessageBox.Show("Konvertierung abgeschlossen" + Environment.NewLine, "TntMPDConverter");
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
			folderBrowserDialog.SelectedPath = edtTargetPath.Text.Length > 0 && edtSourceFile.Text.Length > 0 ? Path.GetDirectoryName(edtSourceFile.Text) : edtTargetPath.Text.Length > 0 ? edtTargetPath.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
			this.label1 = new System.Windows.Forms.Label();
			this.edtSourceFile = new System.Windows.Forms.TextBox();
			this.btnSource = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.edtTargetPath = new System.Windows.Forms.TextBox();
			this.btnTarget = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Projektabrechnung";
			//
			// edtSourceFile
			//
			this.edtSourceFile.Location = new System.Drawing.Point(16, 30);
			this.edtSourceFile.Name = "edtSourceFile";
			this.edtSourceFile.Size = new System.Drawing.Size(230, 20);
			this.edtSourceFile.TabIndex = 1;
			//
			// btnSource
			//
			this.btnSource.Location = new System.Drawing.Point(252, 27);
			this.btnSource.Name = "btnSource";
			this.btnSource.Size = new System.Drawing.Size(28, 23);
			this.btnSource.TabIndex = 2;
			this.btnSource.Text = "...";
			this.btnSource.UseVisualStyleBackColor = true;
			this.btnSource.Click += new System.EventHandler(this.OnSourceClick);
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Zielverzeichnis";
			//
			// edtTargetPath
			//
			this.edtTargetPath.Location = new System.Drawing.Point(16, 74);
			this.edtTargetPath.Name = "edtTargetPath";
			this.edtTargetPath.Size = new System.Drawing.Size(230, 20);
			this.edtTargetPath.TabIndex = 4;
			//
			// btnTarget
			//
			this.btnTarget.Location = new System.Drawing.Point(252, 71);
			this.btnTarget.Name = "btnTarget";
			this.btnTarget.Size = new System.Drawing.Size(28, 23);
			this.btnTarget.TabIndex = 5;
			this.btnTarget.Text = "...";
			this.btnTarget.UseVisualStyleBackColor = true;
			this.btnTarget.Click += new System.EventHandler(this.OnTargetClick);
			//
			// btnOk
			//
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(56, 109);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			//
			// btnCancel
			//
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(146, 109);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Abbrechen";
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			// openFileDialog
			//
			this.openFileDialog.FileName = "Projektabrechnung.txt";
			this.openFileDialog.Filter = "RTF Dateien|*.rtf|Alle Dateien|*.*";
			this.openFileDialog.Title = "Projektabrechnung auswählen";
			//
			// Form1
			//
			this.AcceptButton = this.btnOk;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(292, 148);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnTarget);
			this.Controls.Add(this.edtTargetPath);
			this.Controls.Add(this.btnSource);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.edtSourceFile);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Projektabrechnung konvertieren";
			this.ResumeLayout(false);
			this.PerformLayout();

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

