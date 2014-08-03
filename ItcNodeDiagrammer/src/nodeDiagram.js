/// <reference path="../lib/d3.v3.min.js" />
d3.ItcNodeGraph = d3.ItcNodeGraph || {};

d3.ItcNodeGraph.itcNode = function () {
	var chartStamp;
	var chartNode;
	var chartWidth = 300;
	var chartHeight = 150;
	var heightChartBars = 10;
	var chartX = 20;
	var chartY = 5;
	var g;
	var selectionAction;

	chart.node = function (value) {
		if (!arguments.length) return chartNode;
		chartStamp = value.Stamp;
		chartNode = value;
		return chart;
	};
											
	chart.width = function (value) {
		if (!arguments.length) return chartWidth;
		chartWidth = value;
		return chart;
	};

	chart.height = function (value) {
		if (!arguments.length) return chartHeight;
		chartHeight = value;
		return chart;
	};

	chart.heightOfStampValues = function (value) {
		if (!arguments.length) return heightChartBars;
		heightChartBars = value;
		return chart;
	};

	chart.x = function (value) {
		if (!arguments.length) return chartX;
		chartX = value;
		return chart;
	};

	chart.y = function (value) {
		if (!arguments.length) return chartY;
		chartY = value;
		return chart;
	};
	
	chart.g = function (value) {
		if (!arguments.length) return g;
		g = value;
		return chart;
	};

	chart.selectionAction = function(value) {
		if (!arguments.length) return selectionAction;
		selectionAction = value;
		return chart;
	};

	function chart(group) {
		g = group;
		update();
	}

	function update() {
		var chartId = 'x' + chartNode.Id.split("-").join("");
		g.setAttribute("id", chartId);
		var gWidth = g.attributes['width'].value;
		var gHeight = g.attributes['height'].value;
		g = d3.select("#" + chartId); 
						
		// Background
		g.append("rect")
			.attr({
				stroke: "#111111",
				fill: "#fff",
				x: 0,
				y: 0,
				width: parseInt(gWidth) + (chartX * 2),
				height: parseInt(gHeight) + (chartY * 2),
			})
		.classed("itcNodeBackground", true)
		.on("click", raiseSelectedEvent);


		// ID
		drawId([chartStamp.Id], chartWidth, chartX);
		// Event
		drawEvent(chartStamp.Event, chartWidth, chartY + heightChartBars);


	}

	function raiseSelectedEvent(e) {
		if (selectionAction) {
			selectionAction(this, e);
		}
	};

	function drawEvent(event, startingWidth, startingY) {
		var nesting = 0;
		drawValue(event, chartX, startingY, chartWidth);

		function drawValue(innerEvent, innerStartingX, innerStartingY, innerWidth) {
			nesting++;
			var valueBar = g.append('rect').classed('eventBar' + nesting, true);
			var innerBarHeight = innerEvent.Value * heightChartBars;
			valueBar.attr({
				x: innerStartingX,
				y: innerStartingY,
				width: innerWidth,
				height: innerBarHeight,
				fill: "#5325b0",
				stroke: "#000000"
			});
			if (innerEvent.Left && (innerEvent.Left.Value > 0 | !innerEvent.Left.IsLeaf)) {
				drawValue(innerEvent.Left, innerStartingX, innerStartingY + innerBarHeight, innerWidth / 2);
			}
			if (innerEvent.Right && (innerEvent.Right.Value > 0 | !innerEvent.Right.IsLeaf)) {
				drawValue(innerEvent.Right, innerStartingX + innerWidth / 2, innerStartingY + innerBarHeight, innerWidth / 2);
			}
		}


	}

	function drawId(id, startingWidth, startingX) {
		var nesting = 0;
		innerDrawId(id, startingWidth, startingX);
		function innerDrawId(innerId, innerStartingWidth, innerStartingX) {
			nesting++;
			//console.log(nesting); 
			var idBars = g.selectAll('rect.idBar' + nesting).data(innerId);
			idBars.enter().append('rect').classed('idBar' + nesting, true);
			idBars.attr({
				x: innerStartingX,
				y: chartY,
				fill: function (d) {
					return ((d && d.Value > 0) ? "#7fdfff" : "#fffffe");
				},
				stroke: "#000000",
				height: heightChartBars,
				width: function (d) {
					var barWidth;
					if (d.IsLeaf) {
						//console.log('nesting:' + nesting + ' | w1 | barWidth:' + innerStartingWidth + ' | X:' + innerStartingX + ' | data: (' + d.IsLeaf + ',' + d.Value + ',' + (d.Left ? d.Left.Value : 'null') + ',' + (d.Right ? d.Right.Value : 'null') + ')');
						return innerStartingWidth;
					}
					if ((d.Left) && (d.Right)) {
						barWidth = innerStartingWidth / 2;
						//console.log('nesting:' + nesting + ' | w2 | barWidth:' + barWidth + ' | X:' + innerStartingX + ' | data: (' + d.IsLeaf + ',' + d.Value + ',' + (d.Left ? d.Left.Value : 'null') + ',' + (d.Right ? d.Right.Value : 'null') + ')');
						innerDrawId([d.Left], barWidth, innerStartingX );
						innerDrawId([d.Right], barWidth, innerStartingX + barWidth);
						return barWidth;
					}
					barWidth = innerStartingWidth;
					//console.log('nesting:' + nesting + ' | w3 | barWidth:' + barWidth + ' | X:' + innerStartingX + ' | data: (' + d.IsLeaf + ',' + d.Value + ',' + (d.Left ? d.Left.Value : 'null') + ',' + (d.Right ? d.Right.Value : 'null') + ')');
					if (d.Left) {
						innerDrawId(d.Left, barWidth, innerStartingX);
					} else {
						innerDrawId(d.Right, barWidth, innerStartingX);
					}
					return barWidth;
				}
			});
		}

	}

	function add(svg, itcStamp, width, barHeight, x, y) {
		if (!svg) {
			svg = d3.select('svg');
		}
		if (!itcStamp) {
			throw new Error("An itcStamp must be provided.");
		}
		chartStamp = itcStamp;
		if (width) { chartWidth = width;}
		if (barHeight) { heightChartBars = barHeight;}
		if (x) { chartX = x;}
		if (y) { chartY = y;}
		g = svg.append("g");
		update();
		return chart;
	}

	chart.add = add;
	chart.update = update;
	return chart;
};
