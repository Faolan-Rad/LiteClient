﻿using Foundation;

namespace RhubarbVR
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
		protected override MauiApp CreateMauiApp() {
			return MauiProgram.CreateMauiApp();
		}
	}
}