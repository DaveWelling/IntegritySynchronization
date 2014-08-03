itcNodeApi = new function() {
	var api = {};

	api.newNode = function() {
		return $.getJSON("http://localhost/ItcActionWebApi/api/newnode");
	};

	api.fork = function(node) {
		return $.ajax({
			url: "http://localhost/ItcActionWebApi/api/fork",
			type: "POST",
			data: JSON.stringify(node),
			contentType: "application/json; charset=utf-8",
			dataType: "json"
		});
	};

	api.join = function(node1, node2) {
		var nodes = {
			Node1: node1,
			Node2: node2
		};
		return $.ajax({
			url: "http://localhost/ItcActionWebApi/api/join",
			type: "POST",
			data: JSON.stringify(nodes),
			contentType: "application/json; charset=utf-8",
			dataType: "json"
		});
	};

	api.nodeEvent = function (node) {
		return $.ajax({
			url: "http://localhost/ItcActionWebApi/api/nodeevent",
			type: "POST",
			data: JSON.stringify(node),
			contentType: "application/json; charset=utf-8",
			dataType: "json"
		});
	};

	return api;
}