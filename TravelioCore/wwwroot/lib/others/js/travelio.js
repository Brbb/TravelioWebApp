$(document).ready(function () {
    $(".countryRes").autocomplete({
		source: function (request, response) { 
    		$.ajax({ 
				url: "/Home/GetFilteredCountries",
				type: "POST",
				dataType: "json", 
				data: { prefix: request.term }, 
				success: function (data) { 
    
				response($.map(data, function (value,key) { 
                	return { label: value.name }; 
    				})); 
				} 
			});
	},
	messages : {noResults : '',results : function(){}},
	});
});



//------- FB
  function statusChangeCallback(response) {
    console.log(response);
    if (response.status === 'connected') {
      	$(".btn-signin").text("Sign Out");
      	$(".btn-signin").unbind("click",showLoginModal);
      	$(".btn-signin").bind("click",fbLogout);

      	FB.api('/me?fields=first_name,hometown', function(fbObject) {
	      		travelio.user.first_name = fbObject.first_name;
				travelio.user.country = fbObject.hometown.name.split(",")[1].trim();
				console.log(travelio.user.country);
				setDepartureCountryValueByName(travelio.user.country);
				$(".btn-profile").text(travelio.user.first_name);
		      	$(".btn-profile").show();
	      	});
      	} 
      	else {
    	$(".btn-signin").text("Sign In");
      	$(".btn-signin").bind("click",showLoginModal);
      	$(".btn-signin").unbind("click",fbLogout);
      	$(".btn-profile").text("");
      	$(".btn-profile").hide();
      	console.log('Please log into this app.');
    }
  }


  function showLoginModal(){
  	$("#loginModal").modal('show');
  };

  window.fbAsyncInit = function() {
  FB.init({
    	appId      : '267333063678315',
    	status     : true,
    	cookie     : true,  // enable cookies to allow the server to access 
                        // the session
    	xfbml      : true,  // parse social plugins on this page
    	version    : 'v2.8' // use graph api version 2.8
  	});

  	FB.getLoginStatus(function(response) {
 	   statusChangeCallback(response);
  	});
  };


function fbLogout(){
	FB.logout(function(response) {
	statusChangeCallback(response);
	getLocation(function(data){
				setDepartureCountryValue(data.country_code);
				});
	});
}


function InitUI(){

	$(".btn-login").on('click',function(){
		$("#loginModal").modal('hide');
	});
	
	$("#fbLoginBtn").on('click', function(){

		console.log('Welcome!  Fetching your information.... ');
		FB.login(function(response) {
			if (response.authResponse) {
			//user just authorized your app
				console.log(response);
				statusChangeCallback(response);
			}
		}, {scope: 'email,public_profile,user_hometown', return_scopes: true});
	});
}
//----------


function setDepartureCountryValueByName(departureCountryName)
{
	$.ajax({ 
				url: "/Home/UpdateDepartureCountryByName",
				type: "PUT",
				dataType: "json", 
				data: { name: departureCountryName }, 
				success: function (country) { 
					$(".btn-home-country").text(country.Name);
					$('input#departureCountryName').val(country.Name);
					$('input#passportCountryName').val(country.Name);
					$(".destination-link").attr('href',"Destination/Destination?passportCountryCode="+country.Alpha2Code);
				} 
			});
}

function setDepartureCountryValue(departureCountryCode)
{
	$.ajax({ 
				url: "/Home/UpdateDepartureCountryByCode",
				type: "PUT",
				dataType: "json", 
				data: { code: departureCountryCode }, 
				success: function (country) { 
					$(".btn-home-country").text(country.Name);
					$('input#departureCountryName').val(country.Name);
					$('input#passportCountryName').val(country.Name);
					$(".destination-link").attr('href',"Destination/Destination?passportCountryCode="+country.Alpha2Code);
				} 
			});
}

function checkCountryName(name,callback)
{
	$.ajax({ 
				url: "/Timatic/CheckCountryName",
				type: "POST",
				dataType: "json", 
				data: { countryName: name }, 
				success: callback
			});
}


function getLocation(callback){
	$.ajax( {
  					url: '//freegeoip.net/json/',
  					type: 'POST',
  					dataType: 'jsonp',
  					success: callback
				});
}

function Travelio()
{
	this.user = 
	{
		first_name: "",
		country: ""
	}
}

Travelio.prototype.getUserInfo = function(callback){
		FB.api("/me?fields=first_name,hometown", callback);
	};

Travelio.prototype.api = function(apiUrl, data, callback){
		$.ajax({ 
					url: apiUrl,
					type: "GET",
					dataType: "json", 
					data: data, 
					success: callback(result)
					});
	};

Travelio.prototype.checkCountryName = checkCountryName;

Travelio.prototype.getLocation = getLocation;

Travelio.prototype.setDepartureLocationCode = setDepartureCountryValue;

var travelio = new Travelio();
travelio.version = "0.1";
