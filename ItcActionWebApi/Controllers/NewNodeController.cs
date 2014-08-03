using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IntervalTreeClocksCSharp;
using ItcActionWebApi.Models;

namespace ItcActionWebApi.Controllers
{
	public class NewNodeController : ApiController
	{
		// GET api/<controller>
		public Node Get()
		{
			return new Node(new Stamp(), 1);
		}
	}
}