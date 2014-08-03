using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ItcActionWebApi.Models;

namespace ItcActionWebApi.Controllers
{
	public class NodeEventController : ApiController
	{

		// POST api/<controller>
		public HttpResponseMessage Post(Node nodeWithEvent)
		{
			nodeWithEvent.Stamp.CreateEvent();
			nodeWithEvent.SomeValue0 += 1;
			return Request.CreateResponse(HttpStatusCode.OK, nodeWithEvent);
		}

	}
}