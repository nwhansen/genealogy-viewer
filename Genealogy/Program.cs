//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.IO;
using System.Windows.Forms;

using Genealogy.Model;
using Genealogy.ViewModel;

namespace Genealogy {

	class Program //: Form
	{
		//private static readonly IndividualAttributesFactory attributesFactory = new IndividualAttributesFactory();


		[STAThread]
		static void Main(string[] args) {
			//At this point there really isn't a reason for this
			IndividualManager manager = null;
			if (args.Length > 0 && File.Exists(args[0])) {
				IGenealogyFileInterface fileInterface = new FileInterfaceFactory().CreateFileInterface(args[0]);

				try {
					manager = fileInterface.ParseFile(args[0]);
				} catch (Exception e) {
					MessageBox.Show(e.Message, "Encountered a parsing error!");
					//Terminate
					return;
				}
			}
			new System.Windows.Application().Run(new MainWindow(new MainViewModel(manager)));

		}

	}
}
