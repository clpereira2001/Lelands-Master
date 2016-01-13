using System;

namespace Vauction.Utils.Lib
{
	public class LoggerEventArgs : EventArgs 
	{ 
		#region Private Members
		private readonly string _Exception;
		#endregion

		#region Constructor
		public LoggerEventArgs(string  Exception)
		{
			_Exception = Exception;
		}
		#endregion

		#region Propertys
		public string  Exception
		{
			get
			{
				return _Exception;
			}
		}
		#endregion
	}

	public delegate void ExceptionRaisedEventHandler( LoggerEventArgs e);

}