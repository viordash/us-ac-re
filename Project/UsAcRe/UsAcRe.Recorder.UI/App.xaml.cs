﻿using System;
using System.Windows;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		App() {
			Bootstrapper.Initialize();
			InitializeComponent();
		}

	}
}
