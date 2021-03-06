using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using RazChat.MacClient;
using System.Threading.Tasks;

namespace RazChatMacClient
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
			Client.window = this;
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			Client.Load ();




			inputText.Activated += (object sender, EventArgs e) => {

				string line = inputText.StringValue;

				if (string.IsNullOrWhiteSpace (line)) {
					return;
				}

				if (line.StartsWith ("/")) {
					HandleCommand (line);
				} else {

					AddLineToChatHistory(String.Format("{0}: {1}", Client.sUsername,line));

					Client.SendMessage(line);
				}


				inputText.StringValue = "";
			};
		}

		public void AddLineToChatHistory(string message) {
			textView.TextStorage.Append(new NSAttributedString(message + System.Environment.NewLine));
			textView.ScrollRangeToVisible(new NSRange(textView.TextStorage.ToString().Length, 0));
		}

		private void HandleCommand(string pInput) {
			List<string> splitted = pInput.Substring (1).Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList ();

			if (splitted.Count == 0) {
				return;
			}

			string commandName = splitted [0];

			switch (commandName) {
			case "welcome":
				AddLineToChatHistory(Client.sServer.WelcomeMessage);
				break;
			case "nick":
				if (splitted.Count > 1) {
					Client.updateUsername (splitted [1]);
				} else {
					AddLineToChatHistory ("The correct syntax is /nick <username>");
				}
				break;
			case "help":
				AddLineToChatHistory("/welcome = display the welcome message");
				AddLineToChatHistory("/nick <username> = change your username");
				break;
			}
		}
	}
}

