var Tedious = require("tedious");
var Promise = require("bluebird");

module.exports = {
    msSqlConnecter: function (config) {
        var currentConnect = this;
        currentConnect.config = config;
        currentConnect.errorHandler;
        currentConnect.connectedHandler;
        currentConnect.connection;

        currentConnect.onConnected = function (callback) {
            currentConnect.connectedHandler = callback;
            return currentConnect;
        };

        currentConnect.onError = function (callback) {
            currentConnect.errorHandler = callback;
            return currentConnect;
        };

        currentConnect.Request = function (sql) {
            var currentRequest = this;
            currentRequest.sql = sql;
            currentRequest.params = [];
            currentRequest.result = [];

            currentRequest.errorHandler;
            currentRequest.onComplateHandler;

            currentRequest.addParam = function (key, type, value) {
                currentRequest.params.push({ key: key, type: type, value: value });
                return currentRequest;
            }

            currentRequest.Run = function () {
                var request = new Tedious.Request(currentRequest.sql, function (err, rowCount, rows) {
                    if (err) {
                        currentRequest.errorHandler(err);
                    }
                    else {
                        currentRequest.onComplateHandler(rowCount, currentRequest.result);
                    }
                });

                request.on("row", function (columns) {
                    var item = {};
                    columns.forEach(function (column) {

                        item[column.metadata.colName] = column.value;
                    });
                    currentRequest.result.push(item);
                });

                for (var i in currentRequest.params) {
                    var item = currentRequest.params[i];
                    request.addParameter(item.key, item.type, item.value);
                }

                currentConnect.connection.execSql(request);
                return currentRequest;
            };

            currentRequest.onError = function (callback) {
                currentRequest.errorHandler = callback;
                return currentRequest;
            };

            currentRequest.onComplate = function (callback) {
                currentRequest.onComplateHandler = callback;

                return currentRequest;
            };
        }

        currentConnect.connect = function () {
            var connection = new Tedious.Connection(config);
            currentConnect.connection = connection;
            return Promise.promisify(connection.on.bind(connection))("connect");
        }
    }
}
