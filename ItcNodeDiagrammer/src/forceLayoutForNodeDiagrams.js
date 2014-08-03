/// <reference path="../lib/d3.v3.min.js" />
d3.ItcNodeGraph = d3.ItcNodeGraph || {};

d3.ItcNodeGraph.layout = function () {
	var svg;
	var layoutLinks;
	var layoutNodes;
	var itcNodes = [];
	var nodeDrawer = function () { };
	var linkD3Selection;
	var nodeD3Selection;
	var force;
	var nodeWidth = 50;
	var nodeHeight = 25;

	function layout(hostSvg) {
		svg = hostSvg;

		// TODO: Look at encompasing svg contents into single group and applying
		// zoom and pan to that g instead.
		// Add zooming 
		// create the zoom listener
		//var zoomListener = d3.behavior.zoom()
		//  .scaleExtent([0.1, 3])
		//  .on("zoom", zoomHandler);

		//// function for handling zoom event
		//function zoomHandler() {
		//	d3.select("svg").attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
		//}


		//// apply the zoom behavior to the svg image
		//zoomListener(svg);

		nodeD3Selection = svg.selectAll(".itcNodeShell");
		linkD3Selection = svg.selectAll("line");

		var width = svg[0][0].clientWidth * 1.5;
		var height = svg[0][0].clientHeight * 1.5;
		// Create a force layout for the data described above
		force = d3.layout.force()
			.nodes([])
			.linkDistance(Math.max(nodeHeight, nodeWidth) * 2)
			.size([width, height])
			.gravity(.01)
			.charge(-400);
			//.friction(.7)
			//.distance(150)

		force.on("tick", tick);

		layoutNodes = force.nodes();
		layoutLinks = force.links();

		for (var i = 0; i < itcNodes.length; i++) {
			addItcNodeToLayout(itcNodes[i], i - 1, i);
		}

		restart();
	}

	layout.itcNodes = function (value) {
		if (!arguments.length) return itcNodes;
		itcNodes = value;
		return layout;
	};

	function addItcNode(value, sourceIndex, targetIndex, sourceIndex2) {
		itcNodes.push(value);
		addItcNodeToLayout(value, sourceIndex, targetIndex, sourceIndex2);
		return layout;
	};

	function addItcNodeToLayout(value, sourceIndex, targetIndex, sourceIndex2) {
		if (!sourceIndex && sourceIndex != 0) {
			sourceIndex = itcNodes.length - 2;
		}
		if (!targetIndex && targetIndex != 0) {
			targetIndex = itcNodes.length - 1;
		}	
		if (targetIndex == 0) {
			layoutNodes.push({ x: nodeWidth, y: nodeHeight, fixed: true, itcNode: value });
		} else {
			layoutNodes.push({ x: 0, y: 0, fixed: false, itcNode: value });
			layoutLinks.push({ source: sourceIndex, target: targetIndex });
			if (sourceIndex2) {
				layoutLinks.push({ source: sourceIndex2, target: targetIndex });
			}
		}	   
	}

	layout.addItcNode = addItcNode;
	layout.nodeDrawer = function (value) {
		if (!arguments.length) return nodeDrawer;
		nodeDrawer = value;
		return layout;
	};

	layout.nodeWidth = function (value) {
		if (!arguments.length) return nodeWidth;
		nodeWidth = value;
		return layout;
	};

	layout.nodeHeight = function (value) {
		if (!arguments.length) return nodeHeight;
		nodeHeight = value;
		return layout;
	};
	
	layout.restart = restart;
	function restart() {

		// Draw a line between the nodes of the force layout
		linkD3Selection = linkD3Selection.data(layoutLinks);
		linkD3Selection.enter().insert("line", "line").attr({
			stroke: '#808080'
		});

		// Create a g (svg group) for each entry in the nodesData
		// These g are intended to hold the ITC node graph
		nodeD3Selection = nodeD3Selection.data(layoutNodes);
		nodeD3Selection.enter().insert("g", ".itcNodeShell").classed('itcNodeShell', true)
			.attr({
				width: nodeWidth,
				height: nodeHeight,
				x: 100,
				y: 100,
				fill: '#000'
			})
			.call(force.drag);

		// Put an ITC node graph inside each g
		svg.selectAll('.itcNodeShell').each(function (d, i) {
			if (this.childNodes.length == 0) {
				nodeDrawer(this, d.itcNode);
			}
		});

		force.start();
	}

	// When the force layout gives new coordinates, move the links
	// accordingly and then move the ITC node graph (in the g) using
	// a transform
	function tick(e) {
		// TODO: Make x2/y2 bottom right corner of box
		linkD3Selection
			.attr("x1", function(d) {
				return d.source.x + nodeWidth;
			})
			.attr("y1", function (d) { return d.source.y + nodeHeight; })
			.attr("x2", function (d) { return d.target.x; })
			.attr("y2", function (d) { return d.target.y; });

		nodeD3Selection.attr("transform", function(d) {
			 return "translate(" + (d.x + nodeWidth) + "," + (d.y + nodeHeight) + "), rotate(180)";
		});
		//node.attr("x", function (d) { return d.x; })
		//	.attr("y", function (d) { return d.y; });
	};


	return layout;
}