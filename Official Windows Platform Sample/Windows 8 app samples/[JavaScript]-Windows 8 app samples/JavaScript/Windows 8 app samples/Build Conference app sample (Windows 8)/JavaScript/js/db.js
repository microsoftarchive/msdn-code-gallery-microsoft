/*

    This file is used to persist our cached data using flat files inside of the application local directory.
    This is also where we save things like session notes.

    All data is saved in "collections" which are just Arrays of Objects.

*/

(function () {
    'use strict';

    var db = {
        collections: {}
    };

    var indexCache = {
        collections: {}
    };

    function extendCollection(collection) {
        /// <summary>Add our collection functions onto this array.</summary>

        collection.groupBy = function (key, multiple) {
            /// <summary>Organize the data into groups according to the given key.</summary>
            /// <param name="key" type="String">The property to key off of when grouping objects.</param>
            /// <param name="multiple" type="String">If true, return arrays for each group.
            ///  By default we overwrite if there are multiple values for the grouping.</param>

            var result = {};

            collection.forEach(function (value) {

                var id = value[key];
                if (id === undefined) {
                    return;
                }
                if (multiple) {
                    if (id in result) {
                        result[id].push(value);
                    }
                    else {
                        result[id] = [value];
                    }
                }
                else {
                    result[id] = value;
                }
            });

            return result;
        };

        collection.sortBy = function (property) {
            /// <summary>Sort elements in the array, on the given object property. Strings are compared case-insensitively.</summary>
            return this.sort(function (a, b) {
                var aValue = a[property];
                var bValue = b[property];

                if (typeof (aValue) === "string") {
                    aValue = aValue.toLowerCase();
                }

                if (typeof (bValue) === "string") {
                    bValue = bValue.toLowerCase();
                }

                if (aValue instanceof Date) {
                    aValue = aValue.getTime();
                }

                if (bValue instanceof Date) {
                    bValue = bValue.getTime();
                }

                if (aValue < bValue) {
                    return -1;
                }
                else if (aValue > bValue) {
                    return 1;
                }
                else {
                    return 0;
                }
            });
        };

        collection.filterBy = function (property, value) {
            /// <summary>Filter the collection to only objects who have a named property matching the given value.</summary>
            return extendCollection(this.filter(function (element) {
                if (element[property] === value) {
                    return true;
                }
                else {
                    return false;
                }
            }));
        };

        return collection;
    }

    function init() {
        /// <summary>Read previously saved data from disk.</summary>
        return WinJS.Application.local.readText("db.json", '{"collections":{}}').then(function (data) {
            db = { 'collections': {} };
            try {
                db = ConferenceApp.Util.parseJSON(data);
            }
            catch (e) { }
        }, function (error) {
            ConferenceApp.Debug.log("Error while loading database: " + error);
        });
    }

    function saveAll() {
        /// <summary>Persist data to disk.</summary>
        return WinJS.Application.local.writeText("db.json", JSON.stringify(db));
    }

    function all(collection) {
        /// <summary>Return the named collection. The collection is returned as a clone, so resorting the result array will not
        /// harm other instances, but objects are copied by reference, so modifying objects WILL affect all instances.</summary>
        var results = [];
        if (hasCollection(collection)) {
            results = db.collections[collection];
        }

        return extendCollection(results.slice(0));
    }

    function deleteItem(collectionName, item) {
        /// <summary>Remove an item from a named collection.</summary>
        if (hasCollection(collectionName)) {
            var collection = db.collections[collectionName];
            var index = collection.indexOf(item);
            if (index >= 0) {
                collection.splice(index, 1);
            }
        }
    }

    function replaceItem(collectionName, newItem, id) {
        /// <summary>For the named collection, replace an existing item.</summary>
        /// <param name="collectionName" type="String">The name of the collection.</param>
        /// <param name="newItem" type="Object">The new (updated) object.</param>
        /// <param name="id" type="String">The property of the object to use as its ID. 
        /// Object IDs are expected to be unique within the collection.</param>
        if (hasCollection(collectionName)) {
            var collection = db.collections[collectionName];
            var foundIndex = -1;
            for (var i = 0; i < collection.length; i++) {
                if (collection[i][id] === newItem[id]) {
                    foundIndex = i;
                    break;
                }
            }
            if (foundIndex >= 0) {
                collection.splice(foundIndex, 1, newItem);
            }
            else {
                collection.push(newItem);
            }
        }
    }

    function put(collection, item) {
        /// <summary>Adds an item into a collection. 
        ///     This will create the collection if it does not already exist.</summary>
        if (hasCollection(collection) === false) {
            createCollection(collection, []);
        }
        db.collections[collection].push(item);
    }

    function createCollection(collectionName, data) {
        /// <summary>Creates a new collection.</summary>
        /// <param name="collectionName" type="String">The name of the collection.</param>
        /// <param name="data" type="Array">An Array of Objects to initialize the collection with.</param>
        if (Array.isArray(data)) {
            db.collections[collectionName] = data;
        }
    }

    function hasCollection(collection) {
        /// <summary>Returns true if the collection has already been created.</summary>
        return collection in db.collections;
    }

    function deleteCollection(collection) {
        /// <summary>Deletes an entire collection.</summary>
        return delete db.collections[collection];
    }

    function escapeRegex(s) {
        /// <summary>Escapes things that would normally be part of a regular expression so search literals can contain them.</summary>
        var result = s.replace(/[-[\]{}()*+?.,\\^$|#]/g, "\\$&");
        result = result.replace(/\s+/g, '\\s+');
        return result;
    }

    // searches over all items inside a named collection.
    // config is [{field: 'Title', weight: 10}, {field:['FirstName', LastName'], weight: 5}]
    // returns array of { score: n, item: originalItem, type: type}
    function search(collectionName, config, term, type) {
        var results = [];
        var data = all(collectionName);
        var pattern = new RegExp(escapeRegex(term), "gi");
        data.forEach(function (item) {
            var score = 0;
            config.forEach(function (configOption) {
                var target = '';
                if (Array.isArray(configOption.field)) {
                    var joinedData = [];
                    configOption.field.forEach(function (field) {
                        joinedData.push(item[field]);
                    });
                    target = joinedData.join(' ');
                }
                else {
                    target = item[configOption.field];
                }
                var matches = target.match(pattern);
                if (matches) {
                    score += configOption.weight * matches.length;
                }
            });
            if (score > 0) {
                results.push({ score: score, item: item, type: type });
            }
        });

        return extendCollection(results);
    }

    WinJS.Namespace.define("ConferenceApp.Db", {
        init: init,
        createCollection: createCollection,
        hasCollection: hasCollection,
        deleteCollection: deleteCollection,
        all: all,
        put: put,
        deleteItem: deleteItem,
        replaceItem: replaceItem,
        saveAll: saveAll,
        extendCollection: extendCollection,
        search: search
    });
})();
