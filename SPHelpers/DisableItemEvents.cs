using Microsoft.SharePoint;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
	public class DisableItemEvents : SPItemEventReceiver, IDisposable
	{
		private readonly bool _EventStatus;
		public DisableItemEvents()
		{
			_EventStatus = base.EventFiringEnabled;
			base.EventFiringEnabled = false;
		}

		public void Dispose()
		{
			base.EventFiringEnabled = _EventStatus;
		}
	}
}