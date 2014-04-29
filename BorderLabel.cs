using System;
using System.Drawing;
using System.Windows.Forms;

namespace RanjuSoft.Utilities
{
	/// <summary>
	/// 
	/// </summary>
	public class BorderLabel : System.Windows.Forms.Label
	{
		private Pen m_penBorder = null;

		/// <summary>
		/// 
		/// </summary>
		public BorderLabel()
		{
			m_penBorder = new Pen( Color.Black );
		}

		/// <summary>
		/// 
		/// </summary>
		public Color BorderColor
		{
			get
			{
				return m_penBorder.Color;
			}

			set
			{
				m_penBorder.Color = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			e.Graphics.DrawRectangle( m_penBorder, 0, 0, Bounds.Width - 1, Bounds.Height - 1 );
		}
	}
}
