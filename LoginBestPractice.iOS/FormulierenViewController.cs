using System;
using UIKit;
using DeRoo_iOS;
using System.Threading;


namespace LoginBestPractice.iOS
{
	public partial class FormulierenViewController : UIViewController
	{
		public FormulierenViewController(IntPtr handle) : base(handle)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Thread.Sleep(750);
			RootObject formData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(DataStorage.forms);
			int hoogteVanButtons = 20;

			for (int i = 0; i < formData.formulieren.Count; i++)
			{
				hoogteVanButtons += 40;
				this.View.AddSubview(createElements(formData.formulieren[i].formulier_id, formData.formulieren[i].formulier_naam, hoogteVanButtons));
			}

			base.ViewDidLoad();


			this.NavigationItem.SetRightBarButtonItem(
			new UIBarButtonItem(UIImage.FromFile("logouttemp.png"), UIBarButtonItemStyle.Plain, (sender,args) => 
			{
				var Confirm = new UIAlertView("Uitloggen", "Weet u zeker dat u wilt uitloggen?", null, "Nee", "Ja");
				Confirm.Show();
	            Confirm.Clicked += (object senders, UIButtonEventArgs es) => 
	            {
	               	if (es.ButtonIndex == 1 ) 
					{
						//Create an instance of our AppDelegate
						var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;

						//Get an instance of our MainStoryboard.storyboard
						var mainStoryboard = appDelegate.MainStoryboard;

						//Get an instance of our Login Page View Controller
						var loginPageViewController = appDelegate.GetViewController(mainStoryboard, "LoginPageViewController") as LoginPageViewController;

						//Wire our event handler to show the MainTabBarController after we successfully logged in.
						loginPageViewController.OnLoginSuccess += (s, e) =>
												{
													var tabBarController = appDelegate.GetViewController(mainStoryboard, "MainTabBarController");
													appDelegate.SetRootViewController(tabBarController, true);
												};

						//Set the Login Page as our RootViewController
						appDelegate.SetRootViewController(loginPageViewController, true);
	                }else
	                {
						
	                }
	            };
    		}), true);
		}

		public UIButton createElements(string formulierID, string formulierNaam, int hoogteVanButtons)
		{
			UIButton formulierButton = new UIButton(UIButtonType.RoundedRect);
			formulierButton.SetTitle(formulierNaam, UIControlState.Normal);
			formulierButton.Frame = new CoreGraphics.CGRect(0, hoogteVanButtons, this.View.Frame.Size.Width, 50);
			formulierButton.TouchDown += delegate
			{
				// ViewController //
				FormulierInhoudViewController formulierInhoudController = Storyboard.InstantiateViewController("FormulierInhoud") as FormulierInhoudViewController;
				formulierInhoudController.Title = formulierNaam;
				formulierInhoudController.View.BackgroundColor = UIColor.White;
				NavigationController.PushViewController(formulierInhoudController, true);
				RootObject dataCategorie = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(DataStorage.categories);
				RootObject dataVraag = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(DataStorage.items);

				// ScrollView + Content
				UIScrollView scrollView = new UIScrollView();
				scrollView.Frame = new CoreGraphics.CGRect(0, 0, this.View.Frame.Size.Width, this.View.Frame.Size.Height);
				UIStackView mainStack = new UIStackView();
				mainStack.Axis = UILayoutConstraintAxis.Vertical;

				int catEnVraagHoogte = 0;
				for (int i = 0; i < dataCategorie.categorien.Count; i++)
				{
					if (dataCategorie.categorien[i].formulier_id == formulierID)
					{
						catEnVraagHoogte += 50;
						// cat + vraag // 
						UIStackView catEnVraag = new UIStackView();
						catEnVraag.Axis = UILayoutConstraintAxis.Vertical;
						//catEnVraag.ContentMode = UIViewContentMode.ScaleAspectFit;
						catEnVraag.LayoutMargins = new UIEdgeInsets(0, 0, 30, 0);
						catEnVraag.LayoutMarginsRelativeArrangement = true;;
						catEnVraag.LayoutIfNeeded();

						// categorie // 
						UILabel lbl_cat = new UILabel();
						lbl_cat.ContentMode = UIViewContentMode.ScaleAspectFit;
						lbl_cat.Text = dataCategorie.categorien[i].categorie_text;
						lbl_cat.MinimumFontSize = 12f;
						catEnVraag.AddArrangedSubview(lbl_cat);

						// vraag // 
						for (int j = 0; j < dataVraag.vragen.Count; j++)
						{
							if (dataVraag.vragen[j].categorie_id == dataCategorie.categorien[i].categorie_id)
							{
								UILabel lbl_vraag = new UILabel();
								lbl_cat.ContentMode = UIViewContentMode.ScaleAspectFit;
								lbl_vraag.Text = dataVraag.vragen[j].vraag_text;
								lbl_vraag.Font = UIFont.FromName("Helvetica-Bold", 12f);
								lbl_vraag.AdjustsFontSizeToFitWidth = true;

								// opties //
								UISegmentedControl opties = new UISegmentedControl(); 
								opties.InsertSegment("Akkoord", 0, false);
								opties.InsertSegment("Niet akkoord", 1, false);
								opties.InsertSegment("N.v.t.", 2, false);
								opties.SelectedSegment = 2;
								catEnVraag.AddArrangedSubview(lbl_vraag);
								catEnVraag.AddArrangedSubview(opties);
							}
						}
						mainStack.AddArrangedSubview(catEnVraag);
					}
				}
				// verzendbutton //
				UIButton btn_verzend = new UIButton(UIButtonType.System);
				btn_verzend.SetTitle("Verzend formulier", UIControlState.Normal);
				btn_verzend.ContentMode = UIViewContentMode.ScaleAspectFit;
				mainStack.AddArrangedSubview(btn_verzend);

				// mainStack hoogte // 
				/*
				nfloat mainStackHeight = 0.0f;
				foreach (UIStackView view in mainStack.Subviews)
				{
	   				mainStackHeight += View.Frame.Size.Height;
				}
				*/
				mainStack.Frame = new CoreGraphics.CGRect(0, 0, this.View.Frame.Size.Width, 1200);
				scrollView.ContentSize = mainStack.Frame.Size; 
				scrollView.AddSubview(mainStack);
				//.TouchDown += ;
				scrollView.AddSubview(btn_verzend);  
				formulierInhoudController.View.AddSubview(scrollView);
            };
			return formulierButton;
		}
	}
}