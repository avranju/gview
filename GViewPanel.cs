using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RanjuSoft.Utilities
{
	class GViewPanel : Panel
	{
		private PieData data = null;
		public BorderLabel lblStatus = new BorderLabel();
		private Button btnUp = new Button();
		private Point ptDelta = new Point( 0, 0 );
		private TipForm frmTip = new TipForm();

		private Point BASE_OFFSET_HACK = new Point( 0, 52 );

		public GViewPanel()
		{
			this.SuspendLayout();

			this.SetStyle( ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint,
						   true );

			//
			// initialize label
			//
			lblStatus.BorderColor = Color.FromArgb( 208, 212, 228 );
			lblStatus.BackColor = Color.FromArgb( 238, 242, 255 );
			lblStatus.Size = new System.Drawing.Size( 418, 40 );
			lblStatus.Location = new Point( BASE_OFFSET_HACK.X + 5, BASE_OFFSET_HACK.Y + 2 );
			lblStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			lblStatus.Text = "Ready.";
			this.Controls.Add( lblStatus );

			//
			// initialize button
			//
			btnUp.Text = "U&p";
			btnUp.Location = new Point( 300, 106 );
			btnUp.Size = new Size( 35, 20 );
			btnUp.Enabled = false;
			btnUp.Click += new EventHandler( btnUp_Click );
			this.Controls.Add( btnUp );

			this.ResumeLayout();
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );
			//btnUp.Location = new Point( ClientRectangle.Width - ptDelta.X, ptDelta.Y );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( data == null )
				return;

			//
			// iterate through the child items of the current item
			// and see if this co-ord falls in a region
			//
			bool bFound = false;
			Point pt = new Point( e.X, e.Y );
			Graphics g = CreateGraphics();
			Item subitem = null;

			foreach( Item item in PieData.CurrentItem.SubItems )
			{
				if( item.Region == null )
					continue;

				if( item.Region.IsVisible( pt, g ) )
				{
					subitem = item;
					bFound = true;
					break;
				}
			}

			g.Dispose();

			//
			// ok, item has been found, now descend if the item
			// has sub-items
			//
			if( bFound )
			{
				if( subitem.SubItems.Count > 0 )
				{
					PieData.CurrentItem = subitem;
					btnUp.Enabled = true;
					Refresh();

					lblStatus.Text = FormatCurrentItem();
					lblStatus.Refresh();
				}
			}
		}

		private void btnUp_Click( object sender, EventArgs e )
		{
			PieData.CurrentItem = PieData.CurrentItem.Parent;
			if( PieData.CurrentItem == PieData.Root )
				btnUp.Enabled = false;
			Refresh();

			lblStatus.Text = FormatCurrentItem();
			lblStatus.Refresh();
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( data == null )
				return;

			//
			// iterate through the child items of the current item
			// and see if this co-ord falls in a region
			//
			bool bFound = false;
			Point pt = new Point( e.X, e.Y );
			Graphics g = CreateGraphics();

			foreach( Item item in PieData.CurrentItem.SubItems )
			{
				if( item.Region == null )
					continue;

				if( item.Region.IsVisible( pt, g ) )
				{
					frmTip.Tip = FormatSubItem( item );
					frmTip.Location = PointToScreen( new Point( pt.X, pt.Y - ( frmTip.Size.Height + 15 ) ) );
					if( !frmTip.Visible ) frmTip.Show();
					this.Focus();
					RenderPie( g );
					bFound = true;
					break;
				}
			}

			if( !bFound )
			{
				frmTip.Hide();
				RenderPie( g );
			}

			g.Dispose();
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			Bitmap		bmp = new Bitmap( ClientRectangle.Width, ClientRectangle.Height );
            Graphics	g = Graphics.FromImage( bmp );

			//
			// paint the background colour
			//
			g.FillRectangle( new SolidBrush( Color.FromArgb( 228, 232, 248 ) ),
							 Bounds );

			//
			// if there's nothing to draw then return
			//
			if( data == null )
			{
				//
				// draw the bitmap
				//
				e.Graphics.DrawImage( bmp, 0, 0 );
				e.Graphics.Dispose();
				return;
			}

			//
			// render the pie
			//
			RenderPie( g );
			g.Dispose();

			//
			// draw the bitmap
			//
			e.Graphics.DrawImage( bmp, 0, 0 );
			e.Graphics.Dispose();
			bmp.Dispose();
		}

		private void RenderPie( Graphics g )
		{
			float		fAngle;
			float		fStartAngle = 0;
			Rectangle	rcPie = Rectangle.Inflate( ClientRectangle,
												   -( ( ClientRectangle.Width * 25 ) / 100 ),
												   -( ( ClientRectangle.Height * 25 ) / 100 ) );
			Random		random = new Random();

			//
			// iterate over all the children in PieData.CurrentItem;
			// compute the percent occupied by each child in that item
			// and compute the width of the angle for that item
			//
			foreach( Item item in PieData.CurrentItem.SubItems )
			{
				//
				// compute the angle
				//
				fAngle = (float)( item.Quantity * 360 ) / (float)PieData.CurrentItem.Quantity;

				//
				// create the path with the pie
				//
				GraphicsPath path = new GraphicsPath();
				path.AddPie( rcPie, fStartAngle, fAngle );
				fStartAngle += fAngle;

				//
				// create a region with this path
				//
				if( item.Region != null )
					item.Region.Dispose();
				item.Region = new Region( path );
				path.Dispose();

				//
				// fill the region with a random colour
				//
				item.Color = ( item.Color == Color.Empty ) ? Color.FromArgb( random.Next( 255 ), random.Next( 255 ), random.Next( 255 ) ) : item.Color;
				SolidBrush br = new SolidBrush( item.Color );
				g.FillRegion( br, item.Region );

                br.Dispose();
			}
		}

		private void DisposeItems( Item item )
		{
			if( item.Region != null )
				item.Region.Dispose();

			foreach( Item itm in item.SubItems )
				DisposeItems( itm );
		}

		private string FormatSubItem( Item item )
		{
			System.Diagnostics.Debug.Assert( item.Parent != null );
			return String.Format( "{0}\r\nType: {1}\r\n{2} ({3}%)",
								  item.Name,
								  ( item.SubItems.Count > 0 ) ? "Folder" : "File",
								  FormatSize( item.Quantity ),
								  ( (float)( item.Quantity * 100 ) / (float)item.Parent.Quantity ) );
		}

		private string FormatCurrentItem()
		{
			return String.Format( "{0}\r\nSize: {1}\r\n{2} objects.",
								  PieData.CurrentItem.Name,
								  FormatSize( PieData.CurrentItem.Quantity ),
								  PieData.CurrentItem.SubItems.Count );
		}

		private string FormatSize( long lSize )
		{
			//
			// the input is in bytes; the following logic is followed
			//
			//		-> if the size is less than 1024 bytes then we show in bytes
			//		-> if the size is greater than 1024 but less than 1048576 then we show
			//		   in KB
			//		-> if the size is greater than 1048576 then we show in MB
			//
			if( lSize < 1024 )
				return String.Format( "{0} bytes", lSize );
			else if( lSize < ( 1024 * 1024 ) )
				return String.Format( "{0} KB", ( (float)lSize / 1024 ) );
			else
				return String.Format( "{0} MB", ( (float)lSize / 1024 ) / 1024 );
		}

		public PieData PieData
		{
			set
			{
				if( data != null )
					DisposeItems( data.Root );

				data = value;
				btnUp.Enabled = false;
				Refresh();

				if( data != null )
				{
					lblStatus.Text = FormatCurrentItem();
					lblStatus.Refresh();
				}
			}
			
			get
			{
				return data;
			}
		}
	}

	public class Item
	{
		public string Name;
		public long Quantity;
		public ItemsCollection SubItems;
		public Item Parent;
		public Region Region;
		public Color Color;
		
		public Item()
		{
			Name = string.Empty;
			Quantity = 0;
			SubItems = new ItemsCollection();
			Parent = null;
			Region = null;
			Color = Color.Empty;
		}
	}

	public class PieData
	{
		public Item Root;
		public Item CurrentItem;

		public PieData()
		{
			Root = new Item();
			CurrentItem = Root;
		}
	}
}