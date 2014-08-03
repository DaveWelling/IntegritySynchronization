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
	public class ForkResult
	{
		public ForkResult()
		{

		}
		public ForkResult(Node node1, Node node2)
		{
			Node1 = node1;
			Node2 = node2;
		}

		public Node Node1 { get; set; }
		public Node Node2 { get; set; }
	}
	public class ForkController : ApiController
	{

		// GET api/<controller>/5
		public HttpResponseMessage Post(Node node)
		{
			Stamp stamp2 = node.Stamp.Fork();
			var result = new ForkResult(
				new Node(node.Stamp, node.SomeValue0),
				new Node(stamp2, node.SomeValue0)
				);
			return Request.CreateResponse(HttpStatusCode.OK, result);
		}

	}
}