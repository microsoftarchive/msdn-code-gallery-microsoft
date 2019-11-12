(function () {
    var mountains = [
    {
        id: 0,
        name: "Rainier",
        location: {
            longitude: "-121.760374",
            latitude: "46.852886"
        },

        weatherData: [
        {
            date: formatDateString(new Date().getDate()),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png",
            info: "html/rainier.html"
        },
        {
            date: formatDateString(new Date().getDate() + 1),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 2),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 3),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 4),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 5),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 6),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 7),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 8),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 9),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
    ]
    },
    {
        id: 1,
        name: "St. Helens",
        location: {
            longitude: "-122.1944",
            latitude: "46.1912"
        },

        weatherData: [
        {
            date: formatDateString(new Date().getDate()),
            imgSrc: "images/tile-snow.png",
            hi: "36&deg;",
            low: "12&deg;",
            temp: "32&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "80%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png",
            info: "html/sthelens.html"
        },
        {
            date: formatDateString(new Date().getDate() + 1),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 2),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 3),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 4),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 5),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 6),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 7),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 8),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 9),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
    ]
    },
    {
        id: 2,
        name: "Olympus",
        location: {
            longitude: "-123.710837",
            latitude: "47.801299"
        },
        weatherData: [
        {
            date: formatDateString(new Date().getDate()),
            imgSrc: "images/tile-snow.png",
            hi: "30&deg;",
            low: "20&deg;",
            temp: "28&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "48%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png",
            info: "html/olympus.html"
        },
        {
            date: formatDateString(new Date().getDate() + 1),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 2),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 3),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 4),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 5),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 6),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 7),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 8),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 9),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
    ]
    },
    {
        id: 3,
        name: "Baker",
        location: {
            longitude: "-121.813201",
            latitude: "48.777343"
        },
        weatherData: [
        {
            date: formatDateString(new Date().getDate()),
            imgSrc: "images/tile-snow.png",
            hi: "28&deg;",
            low: "16&deg;",
            temp: "16&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "48%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png",
            info: "html/baker.html"
        },
        {
            date: formatDateString(new Date().getDate() + 1),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 2),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 3),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 4),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 5),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 6),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 7),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 8),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 9),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
    ]
    },
    {
        id: 4,
        name: "Adams",
        location: {
            longitude: "-121.813201",
            latitude: "46.2024"
        },
        weatherData: [
        {
            date: formatDateString(new Date().getDate()),
            imgSrc: "images/tile-snow.png",
            hi: "36&deg;",
            low: "16&deg;",
            temp: "18&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "56%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png",
            info: "html/adams.html"
        },
        {
            date: formatDateString(new Date().getDate() + 1),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 2),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 3),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 4),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 5),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 6),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 7),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 8),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
        {
            date: formatDateString(new Date().getDate() + 9),
            imgSrc: "images/tile-snow.png",
            hi: "32&deg;",
            low: "16&deg;",
            temp: "24&deg;",
            feelsLike: "28&deg;",
            chanceOfSnow: "88%",
            humidity: "64%",
            wind: "18 MPH NW",
            map: "images/placeholder-sdk.png"
        },
    ]
    }];

    function formatDateString(date) {
        var daysOfWeek = [
            "Sun",
            "Mon",
            "Tues",
            "Wed",
            "Thurs",
            "Fri",
            "Sat"
        ];

        var nearDays = [
            "Yesterday",
            "Today",
            "Tomorrow"
        ];

        var dayDelta = date - new Date().getDate() + 1;
        var dateString = "";
        if (dayDelta < 3) {
            dateString = nearDays[dayDelta];
        }
        else {
            dateString = daysOfWeek[date % 7] + " <span class='day'>" + date + "</span>";
        }

        return dateString;
    }

    var dataTable = {
        "Rainier": 0,
        "St. Helens": 1,
        "Olympus": 2,
        "Baker": 3,
        "Adams": 4
    };

    var pageMap = {};

    WinJS.Namespace.define('FluidAppLayout.Data', {
        mountains: mountains,
        dataTable: dataTable,
        pageMap: pageMap
    });

})();
