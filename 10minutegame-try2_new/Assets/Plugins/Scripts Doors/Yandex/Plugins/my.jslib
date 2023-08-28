mergeInto(LibraryManager.library, {

	ShowAdv : function()
	{
		showFullscrAd();
	},

	GetDomainSite : function()
	{
		GetDomainF();
	},

	GetDeviceSDK : function()
	{
		GetDeviceF();
	},

	SaveExtern: function(data) {
    	SaveExternF(UTF8ToString(data));
  	},

  	LoadExtern: function(){
    	LoadExternF();
 	},

	AdvWeapon: function(){
    	AdvWeaponF();
 	},

	GetLang: function () {
		GetLangF();
	}

});