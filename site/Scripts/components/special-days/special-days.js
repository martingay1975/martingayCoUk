/*global define*/
define(['router', 'menuOption', 'text!./special-days.html'], function (router, MenuOption, componentTemplate) {

	"use strict";

	var specialOccassionClick, createSpecialOccassionMenuOption, SpecialDaysViewModel;

	specialOccassionClick = function(calendarTileComponentViewModel) {
		var clickUrl = "#/entries/true/" +
						encodeURIComponent(calendarTileComponentViewModel.menuOption.title) +
						"/" +
						calendarTileComponentViewModel.menuOption.query;

		router.setUrl(clickUrl);
	};

	createSpecialOccassionMenuOption = function (title, imageUrl, query) {
		var id;
		id = title.replace(/\s+/g, '');
		var menuOption = new MenuOption(id, title, specialOccassionClick, imageUrl);
		menuOption.query = query;
		return menuOption;
	};

	SpecialDaysViewModel = function (params) {

		if (!(this instanceof SpecialDaysViewModel)) {
			throw "Must invoke the function SpecialDaysViewModel with the new operator";
		}

		this.trips2020MenuOptions = [];
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Barcelona', 'images/tiles/2020_01.jpg', 'startdatev=20200118&enddatev=20200119'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('North East Trip', 'images/tiles/2020_02.jpg', 'startdatev=20200215&enddatev=20200217'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Ben Skiing', 'images/tiles/2020_02_BenSki.jpg', 'startdatev=20200219&enddatev=20200219'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Mummy Holland', 'images/tiles/2020_03_LauraInHolland.jpg', 'startdatev=20200301&enddatev=20200301'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Cambridge Weekend', 'images/tiles/2020_07_Cambridge.jpg', 'startdatev=20200726&enddatev=20200710'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Boys Oxford 2 Day Bike Ride', 'images/tiles/2020_07_OxfordBikeRide.jpg', 'startdatev=20200726&enddatev=20200711'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Croatia', 'images/tiles/2020_08.jpg', 'startdatev=20200815&enddatev=20200822'));
		this.trips2020MenuOptions.push(createSpecialOccassionMenuOption('Peak District', 'images/tiles/2020_10_PeakDistrict.jpg', 'startdatev=20201025&enddatev=20201028'));

		this.trips2019MenuOptions = [];
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Eastbourne Weekender', 'images/tiles/2019_01.jpg', 'startdatev=20190119&enddatev=20190120'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Liverpool', 'images/tiles/2019_02-Liverpool', 'startdatev=20190219&enddatev=20190221'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Durham', 'images/tiles/2019_02-Durham.jpg', 'startdatev=20190222&enddatev=20190223'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Legoland Hotel', 'images/tiles/2019_02.jpg', 'startdatev=20190209&enddatev=20190209'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('FA Vase Final', 'images/tiles/2019_05.jpg', 'startdatev=20190519&enddatev=20190519'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Paris', 'images/tiles/2019_06.jpg', 'startdatev=20190620&enddatev=20190622'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Thames Kayak', 'images/tiles/2019_06-ThamesKayak', 'startdatev=20190628&enddatev=20190630'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('New Forest Bike', 'images/tiles/2019_07.jpg', 'startdatev=20190713&enddatev=20190715'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Norfolk Camping / Malvern Biking', 'images/tiles/2019_07-NorfolkCamping', 'startdatev=20190727&enddatev=20190802'));
		this.trips2019MenuOptions.push(createSpecialOccassionMenuOption('Lake Garda, Italy', 'images/tiles/2019_08.jpg', 'startdatev=20190814&enddatev=20190821'));

		this.trips2018MenuOptions = [];
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('Baschurch Weekender', 'images/tiles/2018_01.jpg', 'startdatev=20180106&enddatev=20180107'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('Home to Brighton Bike Ride', 'images/tiles/2018_HomeBrightonBikeRide.jpg', 'startdatev=20180428&enddatev=20180429'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('FA Cup Final', 'images/tiles/2018_FACupFinal.jpg', 'id=19-5-2018'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('Denmark', 'images/tiles/2018_05.jpg', 'startdatev=20180526&enddatev=20180530'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('Teneriffe', 'images/tiles/2018_08.jpg', 'startdatev=20180815&enddatev=20180822'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('North Wales', 'images/tiles/2018_10.jpg', 'startdatev=20181020&enddatev=20181023'));
		this.trips2018MenuOptions.push(createSpecialOccassionMenuOption('Teneriffe Cycling', 'images/tiles/2018_11.jpg', 'startdatev=20181109&enddatev=20181113'));

		this.trips2017MenuOptions = [];
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('LA and Vegas', 'images/tiles/2017.jpg', 'startdatev=20171020&enddatev=20171030'));
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('Costa Blanca Cycling', 'images/tiles/2017_CostaBlancaCycling.jpg', 'startdatev=20171006&enddatev=20171009'));
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('Jutland Cycling', 'images/tiles/2017_DenmarkCycling.jpg', 'startdatev=20170812&enddatev=20170816'));
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('Into the Woods', 'images/tiles/2017_IntoTheWoods.jpg', 'id=18-7-2017'));
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('Surrey Police Awards', 'images/tiles/2017_11.jpg', 'id=30-11-2017'));
		this.trips2017MenuOptions.push(createSpecialOccassionMenuOption('Windsor to Winchester Ride', 'images/tiles/2017_WindsorToWinchester.jpg', 'startdatev=20170530&enddatev=20170601'));

		this.trips2016MenuOptions = [];
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Majorca Cycling', 'images/tiles/2016_Majorca.jpg', 'startdatev=20161007&enddatev=20161010'));
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Bournemouth', 'images/tiles/2016_Bournemouth.jpg', 'startdatev=20160806&enddatev=20160813'));
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Camping', 'images/tiles/2016_Camping.jpg', 'startdatev=20160814&enddatev=20160820'));
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Char + Loz Wedding', 'images/tiles/2016_Wedding.jpg', 'startdatev=20160730&enddatev=20160731'));
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Mummy 40th', 'images/tiles/2016_Mummy40th.jpg', 'startdatev=20160624&enddatev=20160626'));
		this.trips2016MenuOptions.push(createSpecialOccassionMenuOption('Shobley Weekender', 'images/tiles/2016_ShobleyWeekender.jpg', 'startdatev=20160520&enddatev=20160522'));

		this.trips2015MenuOptions = [];
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('Manchester 40th', 'images/tiles/2015_Manchester.jpg', 'startdatev=20151024&enddatev=20151027'));
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('Munich Boys', 'images/tiles/2015_Munich.jpg', 'startdatev=20150904&enddatev=20150906'));
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('Gower Camping', 'images/tiles/2015_Gower.jpg', 'startdatev=20150809&enddatev=20150809'));
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('Portugal', 'images/tiles/2015_Portugal.jpg', 'startdatev=20150728&enddatev=20150804'));
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('Scotland Cycling', 'images/tiles/2015_Scotland.jpg', 'startdatev=20150514&enddatev=20150518'));
		this.trips2015MenuOptions.push(createSpecialOccassionMenuOption('French Battlefields', 'images/tiles/2015_Battlefields.jpg', 'startdatev=20150328&enddatev=20150330'));

		this.gayXmasParties = [];
		this.gayXmasParties.push(createSpecialOccassionMenuOption('1998', 'images/years/1998/1998_XX_XX-Spring01', 'id=20-12-1998'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('1999', 'images/years/1999/1999_12_31-PartyRoundJamesBall03', 'id=5-12-1999'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2000', 'images/years/2000/2000_12_XX-XmasParty02.jpg', 'id=9-12-2000'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2001', 'images/years/2001/2001_12_XX-14-HomeXmasParty.jpg', 'id=6-12-2001'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2003', 'images/years/2003/2003_12_06-01-KatieOnBeanBag.jpg', 'id=6-12-2003'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2004', 'images/years/2004/2004_12_18-06-ChristmasParty', 'id=18-12-2004'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2005', 'images/years/2005/2005_12_17-06-ChristmasParty.jpg', 'id=17-12-2005'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2006', 'images/years/2006/2006_12_24-16a-XmasEve', 'id=16-12-2006'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2009', 'images/years/2009/2009_12_13-18a-ChristmasParty.jpg', 'id=13-12-2009'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2011', 'images/years/2011/2011_12_17-08-ChristmasParty.jpg', 'id=17-12-2011'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2012', 'images/years/2012/2012_12_08-13-ChristmasParty.jpg', 'id=8-12-2012'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2014', 'images/years/2014/2014_12_13-19-XmasParty.jpg', 'id=13-12-2014'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2015', 'images/years/2015/2015_12_05-20-XmasParty.jpg', 'id=5-12-2015'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2017', 'images/years/2017/2017_12_09-09-GaySandersElderGlidewell.jpg', 'id=9-12-2017'));
		this.gayXmasParties.push(createSpecialOccassionMenuOption('2018', 'images/years/2018/2018_12_15-02-GaysChristmasParty.jpg', 'id=15-12-2018'));
		
	};

	return { viewModel: SpecialDaysViewModel, template: componentTemplate };
});
