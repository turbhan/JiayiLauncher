﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using JiayiLauncher.Settings;
using JiayiLauncher.Utils;
using static JiayiLauncher.Utils.Imports;

namespace JiayiLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private Mutex? _mutex;
		
	protected override void OnStartup(StartupEventArgs e)
	{
		_mutex = new Mutex(true, "Global\\JiayiLauncher", out var createdNew);
		var args = Environment.GetCommandLineArgs().Skip(1);

		if (createdNew)
		{
			Arguments.Set(string.Join(" ", args));
			return;
		}
		var argString = string.Join(" ", args);
			
		var ptr = Marshal.StringToHGlobalUni(argString);
		
		var hWnd = FindWindowW(null, "Jiayi Launcher");
		if (hWnd != nint.Zero)
		{
			CopyData cds;
			cds.dwData = 1;
			cds.cbData = (uint)((argString.Length + 1) * 2);
			cds.lpData = ptr;
			
			var pCds = Marshal.AllocHGlobal(Marshal.SizeOf<CopyData>());
			Marshal.StructureToPtr(cds, pCds, false);

			SendMessage(hWnd, 0x004A, 0, pCds.ToInt64());
		}
		
		_mutex = null;
		Shutdown();
	}
	
	protected override void OnExit(ExitEventArgs e)
	{
		_mutex?.ReleaseMutex();
		JiayiSettings.Instance!.Save();
		base.OnExit(e);
	}
}