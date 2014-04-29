using System;
using System.Drawing;
using System.Windows.Forms;

namespace RanjuSoft.Utilities
{
	public class TipForm : Form
	{
		private TextBox lblText = new TextBox();

		public TipForm()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ClientSize = new System.Drawing.Size( 318, 150 );

			lblText.Dock = DockStyle.Fill;
			lblText.BackColor = Color.FromArgb( 255, 255, 215 );
			lblText.ReadOnly = true;
			lblText.Multiline = true;
			lblText.BorderStyle = BorderStyle.None;
			this.Controls.Add( lblText );
		}

		public string Tip
		{
			set
			{
				//
				// measure the size required to show the text and adjust
				// the form size accordingly
				//
				using( Graphics g = CreateGraphics() )
				{
					int iChars, iLines;
					SizeF sizeText = g.MeasureString( value,
													  this.Font,
													  new SizeF( 318, 150 ),
													  new StringFormat( StringFormatFlags.NoClip ),
													  out iChars,
													  out iLines );
					this.ClientSize = new Size( (int)sizeText.Width + 20, (int)sizeText.Height );
				}

				lblText.Text = value;
				lblText.Refresh();
			}
		}
	}
}