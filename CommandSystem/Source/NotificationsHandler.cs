using System;

namespace SickDev.CommandSystem
{
	public class NotificationsHandler
	{
		public delegate void OnExceptionThrown(Exception exception);
		public delegate void OnMessageSent(string message);

		public event OnExceptionThrown onExceptionThrown;
		public event OnMessageSent onMessageSent;

		public void NotifyException(Exception exception) => onExceptionThrown?.Invoke(exception);
		public void NotifyMessage(string message) => onMessageSent?.Invoke(message);
	}
}