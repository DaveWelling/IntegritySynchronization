using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Web.Http;
using ItcActionWebApi.Models;
using Sync;

namespace ItcActionWebApi.Controllers
{
	public class JoinController : ApiController
	{
		// POST api/<controller>
		public HttpResponseMessage Post(JoinNodes nodesToJoin)
		{
			Node nodeToReturn = null;
			switch (ChangeEvaluator.GetWhichChanged(nodesToJoin.Node1.Stamp, nodesToJoin.Node2.Stamp))
			{
				case ChangeOccurredIn.Both:
					nodeToReturn = nodesToJoin.Node1;
					nodeToReturn.SomeValue0 = -1;
					nodeToReturn.HasConflict = true;
					nodeToReturn.ConflictValue1 = nodesToJoin.Node1.SomeValue0;
					nodeToReturn.ConflictValue2 = nodesToJoin.Node2.SomeValue0;
					nodeToReturn.Stamp.Join(nodesToJoin.Node2.Stamp);
					break;
				case ChangeOccurredIn.FirstObject:
					nodeToReturn = nodesToJoin.Node1;
					nodeToReturn.Stamp.Join(nodesToJoin.Node2.Stamp);
					break;
				case ChangeOccurredIn.SecondObject:
					nodeToReturn = nodesToJoin.Node2;
					nodeToReturn.Stamp.Join(nodesToJoin.Node1.Stamp);
					break;
				case ChangeOccurredIn.Neither:
					nodeToReturn = nodesToJoin.Node1;
					nodeToReturn.Stamp.Join(nodesToJoin.Node2.Stamp);
					break;
			}

			return Request.CreateResponse(HttpStatusCode.OK, nodeToReturn);
		}

	}

	public class JoinNodes
	{
		public Node Node1 { get; set; }
		public Node Node2 { get; set; }

	}
}