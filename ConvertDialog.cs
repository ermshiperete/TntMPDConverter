// Copyright (c) 2013, Eberhard Beilharz.
// Distributable under the terms of the MIT license (http://opensource.org/licenses/MIT).
using System;
using System.IO;
using Xwt;
using TntMPDConverter.Properties;

namespace TntMPDConverter
{
	public class ConvertDialog: Dialog
	{
		private TextEntry m_SourceFile;
		private TextEntry m_TargetDir;

		public ConvertDialog()
		{
			this.Width = 500;
			this.Title = "TntMPD Converter";
			var vbox = new VBox();
			vbox.PackStart(new Label("Projektabrechnung"));
			var hbox = new HBox();
			m_SourceFile = new TextEntry();
			hbox.PackStart(m_SourceFile, true, true);
			var button = new Button("...");
			button.Clicked += OnSourceClicked;
			hbox.PackEnd(button);
			vbox.PackStart(hbox);
			vbox.PackStart(new Label("Zielverzeichnis"));
			hbox = new HBox();
			m_TargetDir = new TextEntry();
			hbox.PackStart(m_TargetDir, true, true);
			button = new Button("...");
			button.Clicked += OnTargetClicked;
			hbox.PackEnd(button);
			vbox.PackStart(hbox);
			var dialogButton = new DialogButton(Command.Ok);
			dialogButton.Clicked += OnOkClicked;
			Buttons.Add(dialogButton);
			dialogButton = new DialogButton(Command.Cancel);
			Buttons.Add(dialogButton);
			Content = vbox;
		}

		private void OnOkClicked (object sender, EventArgs e)
		{
			if ((m_SourceFile.Text.Length > 0) && (m_TargetDir.Text.Length > 0))
			{
				Settings.Default.SourceFile = m_SourceFile.Text;
				Settings.Default.TargetPath = m_TargetDir.Text;
				var statement = new ConvertStatement();
				statement.DoConversion();
				MessageDialog.ShowMessage("Konvertierung abgeschlossen");
				Settings.Default.Save();
			}
		}

		private void OnSourceClicked(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog("Projektabrechnung ausw\u00e4hlen"))
			{
				dlg.Filters.Add(new FileDialogFilter("RTF-Dateien", "*.rtf"));
				dlg.Filters.Add(new FileDialogFilter("Alle Dateien", "*.*"));
				dlg.InitialFileName = m_SourceFile.Text;
				if (dlg.Run())
				{
					m_SourceFile.Text = dlg.FileName;
					if (m_TargetDir.Text.Length == 0)
						m_TargetDir.Text = Path.GetDirectoryName(dlg.FileName);
				}
			}
		}

		private void OnTargetClicked(object sender, EventArgs e)
		{
			using (var folderBrowserDialog = new SelectFolderDialog("Zielverzeichnis ausw\u00e4hlen"))
			{
				folderBrowserDialog.CanCreateFolders = true;
				folderBrowserDialog.CurrentFolder = m_TargetDir.Text.Length > 0 && m_SourceFile.Text.Length > 0 ? Path.GetDirectoryName(m_SourceFile.Text) : m_TargetDir.Text.Length > 0 ? m_TargetDir.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (folderBrowserDialog.Run())
				{
					m_TargetDir.Text = folderBrowserDialog.CurrentFolder;
				}
			}
		}
	}
}

