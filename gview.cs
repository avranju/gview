using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RanjuSoft.Utilities
{
	public class GViewForm : Form
	{
		//
		// application name
		//
		public const string APPLICATION_NAME = "GView";
	
		//
		// application version
		//
		public const string APPLICATION_VERSION = "1.0";
	
		//
		// panel containing the controls
		//
		private Panel pnlControls;

		//
		// panel containing the pie chart
		//
		private GViewPanel pnlGView;
	
		//
		// text box containing the path to the folder to scan
		//
		private TextBox txtFolderPath = new TextBox();

		//
		// click this button to get the graphic drawn
		//
		private Button btnScanNow = new Button();

		//
		// checking this box causes the application to not update
		// the status label while scanning; results in faster scans
		//
		private CheckBox chkUpdateUI = new CheckBox();

		private PieData _data = null;
		private ComputeState _state = ComputeState.Pending;

		public GViewForm()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			//
			// initialize the form
			//
			this.Text = String.Format( "{0} {1}",
									   GViewForm.APPLICATION_NAME,
									   GViewForm.APPLICATION_VERSION );
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.AcceptButton = btnScanNow;
			this.ClientSize = new Size( 436, 436 );
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

			//
			// initialize the controls panel
			//
			pnlControls = new Panel();
			pnlControls.Size = new Size( pnlControls.Size.Width, txtFolderPath.Height + chkUpdateUI.Height + 4 );
			pnlControls.Dock = DockStyle.Top;
			this.Controls.Add( pnlControls );

			//
			// initialize the folder path text box
			//
			txtFolderPath.Size = new Size( pnlControls.Size.Width - 100,
										   txtFolderPath.Size.Height );
			txtFolderPath.Anchor = AnchorStyles.Right | AnchorStyles.Left;
			txtFolderPath.Text = "C:\\";
			pnlControls.Controls.Add( txtFolderPath );

			//
			// initialize the scan now button
			//
			btnScanNow.Text = "&Scan Now!";
			btnScanNow.Location = new Point( txtFolderPath.Size.Width + 3,
											 txtFolderPath.Location.Y );
			btnScanNow.Size = new Size( btnScanNow.Size.Width + 20, txtFolderPath.Size.Height );
			btnScanNow.Anchor = AnchorStyles.Right;
			pnlControls.Controls.Add( btnScanNow );

			//
			// initialize the check box
			//
			chkUpdateUI.Text = "&Update UI while scanning (uncheck for faster scans)";
			chkUpdateUI.Location = new Point( 3, txtFolderPath.Size.Height );
			chkUpdateUI.Size = new Size( txtFolderPath.Size.Width + 70, chkUpdateUI.Size.Height );
			chkUpdateUI.Checked = true;
			pnlControls.Controls.Add( chkUpdateUI );

			//
			// initialize the graphical view panel
			//
			pnlGView = new GViewPanel();
			pnlGView.Dock = DockStyle.Fill;
			this.Controls.Add( pnlGView );

			//
			// register events
			//
			this.SizeChanged += new EventHandler( GViewForm_SizeChanged );
			btnScanNow.Click += new EventHandler( btnScanNow_Click );
		}

		private void btnScanNow_Click( object sender, EventArgs e )
		{
			switch( _state )
			{
				case ComputeState.Pending:
					//
					// see that the path is valid
					//
					if( ( txtFolderPath.Text == string.Empty ) ||
						!( Directory.Exists( txtFolderPath.Text ) ) )
					{
						MessageBox.Show( "Please specify a valid path to a folder.",
									 	 "GView - Error",
									 	 MessageBoxButtons.OK,
									 	 MessageBoxIcon.Error );
						txtFolderPath.Focus();
						return;
					}

					//
					// update our state
					//
					_state = ComputeState.Computing;
					btnScanNow.Text = "&Stop";

					//
					// initialize the pie data
					//
					pnlGView.PieData = null;
					_data = new PieData();
					ComputeSizeDelegate computeSize = new ComputeSizeDelegate( ComputeSize );
					computeSize.BeginInvoke( txtFolderPath.Text,
											 _data.Root,
											 new AsyncCallback( OnEndComputeSize ),
											 null );
					break;

				case ComputeState.Computing:
					//
					// update the shared "_state" variable; the worker thread
					// should get notified of this some time in the future
					//
					_state = ComputeState.Cancelled;

					//
					// disable the button till the fact of the process having
					// been cancelled dawns on the worker thread
					//
					btnScanNow.Enabled = false;
					break;

				case ComputeState.Cancelled:
					//
					// we disable the button once we enter this state;
					// so if the user was still able to click it then the
					// system must be on fire or something
					//
					System.Diagnostics.Debug.Assert( false );
					break;
			}
		}

		private void OnEndComputeSize( IAsyncResult result )
		{
			SetPieData();
		}

		delegate void SetPieDataDelegate();

		private void SetPieData()
		{
			//
			// check if we are running on the UI thread
			//
			if( this.InvokeRequired == false )
			{
				//
				// see if the process was cancelled
				//
				if( _state == ComputeState.Cancelled )
					ShowStatus( "Ready." );

				_state = ComputeState.Pending;

				//
				// if the user has cancelled the process then we show
				// whatever we have scanned in so far; to guard against
				// showing nothing to the user, we check to see if
				// atleast the name of the root item has been set; if
				// yes, we are good to go!
				//
				if( _data.Root.Name != string.Empty )
					pnlGView.PieData = _data;

				btnScanNow.Text = "&Scan Now!";
				btnScanNow.Enabled = true;
			}
			else
			{
				//
				// we aren't on the UI thread so we call ourselves on the UI thread
				//
				SetPieDataDelegate setPieData = new SetPieDataDelegate( SetPieData );
				this.Invoke( setPieData, null );
			}
		}

		//
		// delegate to begin asynch processing of files
		//
		delegate void ComputeSizeDelegate( string strFolderPath, Item item );

		private void ComputeSize( string strFolderPath, Item item )
		{
			try
			{
				string[] arrFiles = Directory.GetFiles( strFolderPath );
				string[] arrFolders = Directory.GetDirectories( strFolderPath );
				item.Name = strFolderPath;

				//
				// sum up the size of all the files
				//
				foreach( string strFile in arrFiles )
				{
					//
					// show status if user wants it
					//
					if( chkUpdateUI.Checked )
						ShowStatus( strFile );

					//
					// get out of here if we've been asked to cancel
					// our job
					//
					if( _state == ComputeState.Cancelled ) return;

					try
					{
						Item subitem = new Item();
						subitem.Name = strFile;
						subitem.Parent = item;
						subitem.Quantity = ( new FileInfo( strFile ).Length );

						item.SubItems.Add( subitem );
						item.Quantity += subitem.Quantity;
					}
					catch( Exception ) {}
				}
				
				//
				// recursively sum up the size of all the folders
				//
				foreach( string strFolder in arrFolders )
				{
					//
					// get out of here if we've been asked to cancel
					// our job
					//
					if( _state == ComputeState.Cancelled ) return;

					Item subitem = new Item();
					subitem.Parent = item;

					ComputeSize( strFolder, subitem );
					item.Quantity += subitem.Quantity;
					item.SubItems.Add( subitem );
				}
			}
			catch( Exception )
			{
			}
		}

		delegate void ShowStatusDelegate( string strText );

		private void ShowStatus( string strText )
		{
			//
			// check if we are running on the UI thread
			//
			if( this.InvokeRequired == false )
			{
				pnlGView.lblStatus.Text = strText;
				pnlGView.lblStatus.Refresh();
			}
			else
			{
				//
				// we aren't on the UI thread so we call ourselves on the UI thread
				//
				ShowStatusDelegate showStatus = new ShowStatusDelegate( ShowStatus );
				this.Invoke( showStatus, new object[] { strText } );
			}
		}

		private void GViewForm_SizeChanged( object sender, EventArgs e )
		{
			this.Text = String.Format( "{0} {1} ({2}x{3})",
									   GViewForm.APPLICATION_NAME,
									   GViewForm.APPLICATION_VERSION,
									   this.Size.Width,
									   this.Size.Height );
		}

		public static void Main()
		{
			try
			{
				Application.Run( new GViewForm() );
			}
			catch( Exception ex )
			{
				MessageBox.Show( ex.Message, "ERROR" );
			}
		}
	}

	enum ComputeState
	{
		Pending,			// compute is not in progress
		Computing,			// compute is in progress
		Cancelled			// compute has been cancelled
	}
}