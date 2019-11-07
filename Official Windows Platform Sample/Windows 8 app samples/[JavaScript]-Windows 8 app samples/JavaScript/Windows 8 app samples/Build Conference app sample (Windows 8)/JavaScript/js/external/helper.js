/*

    This file contains various helper methods that let you modify the DOM using a fluent pattern.
    It also implements a subset of Mustache templating, with some modifications.

*/

(function (undefinedVal) {
    "use strict";

    var templateCache = {};

    //** Query object **//

    var QueryCollection = function (items) {
        this.add(items);
    };

    QueryCollection.prototype = Object.create(Array.prototype);


    QueryCollection.prototype.text = function (text) {
        /// <summary>Sets the textContent property of the selected nodes.</summary>
        if (text === undefinedVal) {
            if (this.length > 0) {
                return this[0].textContent;
            }
            return "";
        }
        else {
            this.forEach(function (item) {
                item.textContent = text;
            });
            return this;
        }
    };

    QueryCollection.prototype.html = function (html) {
        /// <summary>Sets the innerHTML property of the selected nodes.</summary>
        if (html === undefinedVal) {
            if (this.length > 0) {
                return this[0].html;
            }
            return "";
        }
        else {
            this.forEach(function (item) {
                item.innerHTML = html;
            });
            return this;
        }
    };

    QueryCollection.prototype.hasClass = function (className) {
        /// <summary>Calls the WinJS.Utilities.hasClass helper on the first selected node.</summary>

        if (this.length > 0) {
            return WinJS.Utilities.hasClass(this[0], className);
        }
        return false;
    };

    QueryCollection.prototype.addClass = function (className) {
        /// <summary>Calls the WinJS.Utilities.addClass helper on the selected nodes.</summary>
        this.forEach(function (item) {
            WinJS.Utilities.addClass(item, className);
        });
        return this;
    };

    QueryCollection.prototype.removeClass = function (className) {
        /// <summary>Calls the WinJS.Utilities.removeClass helper on the selected nodes.</summary>
        this.forEach(function (item) {
            WinJS.Utilities.removeClass(item, className);
        });
        return this;
    };

    QueryCollection.prototype.toggleClass = function (className) {
        /// <summary>Calls the WinJS.Utilities.toggleClass helper on the selected nodes.</summary>
        this.forEach(function (item) {
            WinJS.Utilities.toggleClass(item, className);
        });
        return this;
    };

    QueryCollection.prototype.empty = function () {
        /// <summary>Calls the WinJS.Utilities.empty helper on the selected nodes.</summary>
        this.forEach(function (item) {
            WinJS.Utilities.empty(item);
        });
        return this;
    };

    QueryCollection.prototype.remove = function () {
        /// <summary>Removes the selected nodes from the DOM.</summary>
        this.forEach(function (item) {
            item.parentNode.removeChild(item);
        });
        return this;
    };

    QueryCollection.prototype.click = function (clickHandler) {
        /// <summary>Adds the given handler as a listener for the click event.</summary>
        this.forEach(function (item) {
            item.addEventListener('click', clickHandler, false);
        });
        return this;
    };

    QueryCollection.prototype.listen = function (eventName, handler, capture) {
        /// <summary>Listens for the named DOM event using the given handler for each of the selected nodes.</summary>
        capture = capture || false;
        this.forEach(function (item) {
            item.addEventListener(eventName, handler, capture);
        });
        return this;
    };

    QueryCollection.prototype.unlisten = function (eventName, handler, capture) {
        /// <summary>Removes a DOM event listener on each selected node.</summary>
        capture = capture || false;
        this.forEach(function (item) {
            item.removeEventListener(eventName, handler, capture);
        });
        return this;
    };

    QueryCollection.prototype.appendTo = function (element) {
        /// <summary>Inserts the selected nodes as the last child (in order) of the given element</summary>
        /// <param name="element">Either the string of a query selector, a QueryCollection object, or a HTMLElement.</param>
        if (typeof (element) === "string") {
            element = $(element)[0];
        }
        else if (element instanceof QueryCollection) {
            element = element[0];
        }

        if (element) {
            this.forEach(function (item) {
                element.appendChild(item);
            });
        }
        return this;
    };

    QueryCollection.prototype.append = function (element) {
        /// <summary>Appends the given element (or set of elements) as the child of the first selected node.</summary>
        /// <param name="element">Either the string of a query selector, a QueryCollection object, a HTMLElement, or an array of HTMLElements.</param>

        var that = this;

        if (typeof (element) === "string") {
            element = $(element);
        }

        if (this.length > 0) {
            if (element.forEach) {
                element.forEach(function (item) {
                    that[0].appendChild(item);
                });
            }
            else {
                that[0].appendChild(element);
            }
        }

        return this;
    };


    QueryCollection.prototype.attr = function (attributeName, attributeValue) {
        /// <signature>
        /// <summary>Sets the given attribute on the selected nodes</summary>
        /// <param name="attributeName" type="String">The name of the HTML attribute to set.</param>
        /// <param name="attributeValue" type="String">The value to set the HTML attribute to.</param>
        /// </signature>
        /// <signature>
        /// <summary>Sets the given attributes on the selected nodes</summary>
        /// <param name="attributeObj" type="Object">An object that specifies attribute/value pairs to set. 
        ///     e.g. {'aria-name': 'Main Navigation'}.</param>
        /// </signature>
        /// <signature>
        /// <summary>Gets the value for the named attribute on the first selected node.</summary>
        /// <param name="attributeName" type="String">The name of the HTML attribute to get.</param>
        /// </signature>
        if (attributeName) {
            if (typeof (attributeName) === 'object') {
                for (var attribute in attributeName) {
                    var value = attributeName[attribute];
                    this.forEach(function (item) {
                        item.setAttribute(attribute, value);
                    });
                }
            }
            else if (attributeValue) {
                this.forEach(function (item) {
                    item.setAttribute(attributeName, attributeValue);
                });
            }
            else if (this.length > 0) {
                return this[0].getAttribute(attributeName);
            }
        }
        return this;
    };

    QueryCollection.prototype.removeAttr = function (attribute) {
        /// <summary>Removes the listed attribute from each selected node.</summary>
        var that = this;
        if (attribute) {
            this.forEach(function (item) {
                item.removeAttribute(attribute);
            });
        }
        return this;
    };

    QueryCollection.prototype.add = function (items) {
        /// <signature>
        /// <summary>Adds nodes into the current QueryCollection.</summary>
        /// <param name="items" type="Array">An array or QueryCollection of HTMLElements.</param>
        /// </signature>
        /// <signature>
        /// <summary>Adds a node into the current QueryCollection.</summary>
        /// <param name="item" type="HTMLElement">The node to add.</param>
        /// </signature>
        if (items) {
            if (!('length' in items) || items instanceof HTMLElement) {
                this.push(items);
            }
            else {
                for (var i = 0; i < items.length; i++) {
                    this.push(items[i]);
                }
            }
        }
    };

    QueryCollection.prototype.find = function (query) {
        /// <summary>Applies the given query to each currently selected node and returns the composite selected set of children.</summary>
        /// <param name="query" type="String">A string in DOM querySelector (CSS) syntax</param>
        var results = new QueryCollection();
        if (query) {
            this.forEach(function (item) {
                results.add(item.querySelectorAll(query));
            });
        }
        return results;
    };

    //** end Query object **//


    //** Templating **//

    // searches the data object for the key named 'identifier'.
    // If the value is a fuction, apply it to the data and return the result
    // If the value is null or undefined, return the empty string
    // Otherwise return the found value
    function getValue(data, identifier) {
        var value = trim(identifier).split(".").reduce(function (scope, part) {
            if (scope) {
                if (part in scope) {
                    return scope[part];
                }
            }
            return "";
        }, data);

        if (typeof (value) === 'function') {
            value = value.apply(data);
        }

        if (value === undefinedVal) {
            return "";
        }
        else if (value === null) {
            return "";
        }

        return value;
    }

    // returns true for non empty arrays, Objects with at least one enumerable own property, 
    // or anything that coerces to true after applying a double negation
    function isTruthy(value) {
        if (Array.isArray(value)) {
            return value.length > 0;
        }
        else if (typeof (value) === 'object' && value !== null) {
            return Object.keys(value).length > 0;
        }
        else {
            return !!value;
        }
    }

    // This is used to coerce array members into data objects for getValue
    function makeDataObject(value) {
        if (typeof (value) === 'object') {
            return value;
        }
        else {
            return { '~': value };
        }
    }

    // Trim whitespace around a string
    function trim(str) {
        return str.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    }

    // Escape a string for inserting into HTML.
    // Sanitizes &<>"\ with literal equivalents.
    function escapeString(str) {

        if (typeof (str) !== 'string') {
            return str;
        }

        var escapeChars = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            '\'': '&#39;'
        };
        return str.replace(/[&<>"']/g, function (symbol) {
            return escapeChars[symbol];
        });
    }

    // Perform an XHR for a given template file.
    function getFileContents(path, noCache) {
        var xhr = new XMLHttpRequest();
        if (!noCache) {
            if (path in templateCache) {
                return templateCache[path];
            }
        }
        xhr.open('GET', path, false);
        xhr.send(null);
        if (xhr.status === 200) {
            if (!noCache) {
                templateCache[path] = xhr.responseText;
            }
            return xhr.responseText;
        }
        return "";
    }

    // Render the given mustache template with the provided data.
    function template(template_path, data) {
        var templateString = getFileContents(template_path);
        return render(templateString, data);
    }

    // take a string and treat it like a mustache template
    function render(templateString, data) {
        var root = parse(templateString);
        return root.render(data);
    }

    // replace a mustache tag with the value in the data object, escaped for HTML unless this tag is a triple mustache
    function renderTags(templateString, data) {
        var tagPattern = /{{({)?([^}]*?)(?:\1|})?}}/g;
        return templateString.replace(tagPattern, function (tag, type, key) {
            var value = getValue(data, key);

            switch (type) {
                // triple mustache skips escaping
                case "{":
                    return value;
                default:
                    return escapeString(value);
            }
        });

    }

    // Wrapper object for dealing with a section (a nested mustache block)
    function Section(kind, type, identifier) {
        this.kind = kind;
        this.type = type;
        this.identifier = identifier;
        this.children = [];
    }

    // Turn this section into final markup
    Section.prototype.render = function (data) {
        if (this.kind === 'text') {
            return renderTags(this.identifier, data);
        }
        else if (this.kind === 'content') {

            var value = getValue(data, this.identifier);
            var shouldRender = isTruthy(value);
            var conditionalSection = false;
            if (this.type === '^') {
                shouldRender = !shouldRender;
            }
            else if (this.type === '@') {
                conditionalSection = true;
            }

            if (shouldRender) {

                if (conditionalSection) {
                    return this.renderChildren(data);
                }
                else {
                    if (Array.isArray(value)) {
                        var that = this;
                        var rendered = value.map(function (item) {
                            return that.renderChildren(makeDataObject(item));
                        });
                        return rendered.join('');
                    }
                    else if (typeof (value) === 'object') {
                        return this.renderChildren(value);
                    }
                    else {
                        return this.renderChildren(data);
                    }
                }
            }

            return '';

        }
        else if (this.kind === 'root') {
            return this.renderChildren(data);
        }
        else {
            return '';
        }
    };

    // Render nested sections
    Section.prototype.renderChildren = function (data) {
        var renderedChildren = this.children.map(function (item) {
            return item.render(data);
        });
        return renderedChildren.join('');
    };

    // Take a template string and split it up into sections.
    function tokenize(templateString) {
        var sectionPattern = /({{[#|^|@|\/].*?}})/g;
        return templateString.split(sectionPattern);
    }

    // Take a template string and turn it into a tree of Section objects
    function parse(templateString) {
        var tokens = tokenize(templateString);
        var sectionPattern = /{{([#|^|@|\/])(.*?)}}/g;
        var root = new Section('root', 'root', 'root');
        var openElements = [root];
        var current = root;

        tokens.forEach(function (token) {
            var match = sectionPattern.exec(token);
            if (match) {
                var type = match[1];
                var identifier = match[2];
                switch (type) {
                    case '#':
                    case '^':
                    case '@':
                        // start tag
                        var newSection = new Section('content', type, identifier);
                        current.children.push(newSection);
                        openElements.push(newSection);
                        current = newSection;
                        break;
                    case '/':
                        // end tag
                        if (current.kind === 'content' && current.identifier === identifier) {
                            openElements.pop();
                            current = openElements[openElements.length - 1];
                        }
                        else {
                            throw new Error("Parse error -- unmatched closing tag : " + identifier);
                        }
                        break;
                    default:
                        throw new Error("Parse error -- unknown tag type: " + type);
                }

            }
            else {
                current.children.push(new Section('text', 'text', token));
            }
        });

        if (openElements.length !== 1 || openElements[0].identifier !== 'root') {
            throw new Error("Parse error -- unclosed tags: " + JSON.stringify(openElements.slice(1)));
        }

        return root;
    }

    //** end templating **//

    var externalFunction = function (selector, args) {
        /// <signature>
        /// <summary>Selects nodes from the DOM.</summary>
        /// <param name="selector" type="String">A query selector string to match on.</param>
        /// </signature>
        /// <signature>
        /// <summary>Create a QueryCollection out of the given element.</summary>
        /// <param name="element" type="HTMLElement">Element to create a collection out of.</param>
        /// </signature>
        /// <signature>
        /// <summary>Creates a new node dynamically.</summary>
        /// <param name="element" type="String">The element to create using syntax like &lt;div&gt;.</param>
        /// <param name="attributes" type="Object">Default attributes for the newly created element (see attr).</param>
        /// </signature>
        var idQuery = /^#[A-Za-z0-9_-]+$/;
        var elementCreator = /^<([A-Za-z0-9]+)>$/;

        if (selector instanceof HTMLElement) {
            return new QueryCollection(selector);
        }

        if (idQuery.test(selector)) {
            return new QueryCollection(document.getElementById(selector.slice(1)));
        }
        else {
            var match = elementCreator.exec(selector);
            if (match) {
                return new QueryCollection(document.createElement(match[1])).attr(args);
            }
        }
        return new QueryCollection(document.querySelectorAll(selector));
    };

    externalFunction.render = render;
    externalFunction.template = template;
    externalFunction.getFileContents = getFileContents;
    externalFunction.internal = {};
    externalFunction.internal.tokenize = tokenize;
    externalFunction.internal.parse = parse;

    window.$ = externalFunction;

})();