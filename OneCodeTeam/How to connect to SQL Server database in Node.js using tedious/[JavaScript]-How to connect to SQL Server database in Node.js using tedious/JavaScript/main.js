var TYPES = require("tedious").TYPES;
var msSqlConnecter = require("./msSqlConnecter");

var config = {
    userName: "{your database user name}",
    password: "{your database password}",
    server: "{you database server address}",
    options: {
        database: "{you database name}",
        encrypt: true,
    }
};

function insert(callback) {
    //when insert
    var con = new msSqlConnecter.msSqlConnecter(config);
    con.connect().then(function () {
        new con.Request("insert into student values(@name,@age)")
            .addParam("name", TYPES.VarChar, "Eric")
            .addParam("age", TYPES.Int, 20)
            .onComplate(function (count) {
                if (callback)
                    callback(count);
            })
            .onError(function (err) {
                console.log(err);
            })
            .Run();
    }).catch(function (ex) {
        console.log(ex);
    });
}

function queryAll(callback) {
    var con = new msSqlConnecter.msSqlConnecter(config);
    con.connect().then(function () {
        new con.Request("select * from student")
            .onComplate(function (count, datas) {
                if (callback)
                    callback(datas);
            })
            .onError(function (err) {
                console.log(err);
            }).Run();
    }).catch(function (ex) {
        console.log(ex);
    });
}

function updateData(callback) {
    var con = new msSqlConnecter.msSqlConnecter(config);
    con.connect().then(function () {
        new con.Request("update student set name = @name where id > @id")
            .addParam("id", TYPES.Int, 3)
            .addParam("name", TYPES.VarChar, "frank")
            .onComplate(function (count) {
                if (callback)
                    callback(count);
            })
            .onError(function (err) {
                console.log(err);
            })
            .Run();
    }).catch(function (ex) {
        console.log(ex);
    });
}

function deleteData(callback) {
    var con = new msSqlConnecter.msSqlConnecter(config);
    con.connect().then(function () {
        new con.Request("delete from student where id > @id")
            .addParam("id", TYPES.Int, 3)
            .onComplate(function (count) {
                if (callback)
                    callback(count);
            })
            .onError(function (err) {
                console.log(err);
            })
            .Run();
    }).catch(function (ex) {
        console.log(ex);
    });
}

console.log("inserting data");
insert(function (count) {
    console.log("data inserted, listing data.");
    queryAll(function (data) {
        console.log(data);
        console.log("updating data.");
        updateData(function (count) {
            console.log("data updated, listing data.");
            queryAll(function (data) {
                console.log(data);
                console.log("deleting data.");
                deleteData(function (count) {
                    console.log("data deleted,listing data.");
                    queryAll(function (data) {
                        console.log(data);
                    });
                });
            });
        });
    });
});