using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
	public class DisableItemEvents : SPItemEventReceiver, IDisposable
	{
		private bool _EventStatus;
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