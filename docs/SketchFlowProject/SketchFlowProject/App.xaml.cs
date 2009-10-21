using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

//
// SketchFlow needs to know which control assembly contains its screens. This is set automatically
// on project creation, but if you change the name of the control assembly manually, you must also
// update it manually here.
//
[assembly: Microsoft.Expression.Prototyping.Services.SketchFlowLibraries("SketchFlowProject.Screens")]

namespace SketchFlowProject
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			this.Startup += this.App_Startup;
		}

		private void App_Startup(object sender, StartupEventArgs e)
		{
			this.StartupUri = new Uri(@"pack://application:,,,/Microsoft.Expression.Prototyping.Runtime;Component/WPF/Workspace/PlayerWindow.xaml");
		}
	}
}