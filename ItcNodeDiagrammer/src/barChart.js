var data = [50, 100, 150, 200]
var divs = d3.select('#events .right')
  .selectAll('div.item')
  .data(data);
divs.enter()
  .append('div').classed('item', true)
divs.style({
	width: '20px',
	height: function (d) { return d + 'px' },
	margin: '15px',
	float: 'left',
	'background-color': '#25B0B0'
})
.on('click', function (d, i) {
	d3.select(this)
	  .style('background-color', '#42efef')
	  .style('padding-left', '10px')
	  .text(i)
});