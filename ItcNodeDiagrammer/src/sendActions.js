(function () {
	

	$("#sendAction").click(function() {
		var actionType = $('input[name=ItcAction]:radio:checked').val();
		console.log(actionType);
	});


})();