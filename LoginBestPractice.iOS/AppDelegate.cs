﻿using System;
using System.IO;
using Foundation;
using UIKit;

namespace LoginBestPractice.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

		//
        // public property to access our MainStoryboard.storyboard file
		//
        public UIStoryboard MainStoryboard
        {
            get { return UIStoryboard.FromName("MainStoryboard", NSBundle.MainBundle); }
        }

		//
        // creates an instance of viewControllerName from storyboard
		//
        public UIViewController GetViewController(UIStoryboard storyboard, string viewControllerName)
        {
            return storyboard.InstantiateViewController(viewControllerName);
        }

		//
        // sets the RootViewController of the Apps main window with an option for animation
		//
        public void SetRootViewController(UIViewController rootViewController, bool animate)
        {
            if(animate)
            {
                var transitionType = UIViewAnimationOptions.TransitionFlipFromRight;

                Window.RootViewController = rootViewController;
                UIView.Transition(Window, 0.5, transitionType,
                                  () => Window.RootViewController = rootViewController,
                                  null);
            }
            else
            {
                Window.RootViewController = rootViewController;
            }
        }

		//
        // override FinishedLaunching. This executes after the app has started
		//
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
			//check exists
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var filename = Path.Combine(documents, "login.txt");

			//isAuthenticated can be used for an auto-login feature, you'll have to implement this
			//as you see fit or get rid of the if statement if you want.
			if (File.Exists(filename) == true)
            {
                //We are already authenticated, so go to the main tab bar controller;
                var tabBarController = GetViewController(MainStoryboard, "MainTabBarController");
                SetRootViewController(tabBarController, false);
            }
            else
            {
                //User needs to log in, so show the Login View Controlller
                var loginViewController = GetViewController(MainStoryboard, "LoginPageViewController") as LoginPageViewController;
				loginViewController.OnLoginSuccess += LoginViewController_OnLoginSuccess;

                SetRootViewController(loginViewController, false);
            }
                
            return true;
        }

		//
		// lets user in by showing main view
		//
        void LoginViewController_OnLoginSuccess (object sender, EventArgs e)
        {
            //We have successfully Logged In
            var tabBarController = GetViewController(MainStoryboard, "MainTabBarController");
            SetRootViewController(tabBarController, true);
        }
    }
}

