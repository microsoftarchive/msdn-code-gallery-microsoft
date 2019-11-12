(function() {
  "use strict";

  // Sample title
  var sampleTitle = "Contact Picker Sample";

  // Scenarios
  var scenarios = [{
    url: "/html/Scenario1_PickContact.html",
    title: "Pick a contact"
  }, {
    url: "/html/Scenario2_PickContacts.html",
    title: "Pick multiple contacts"
  }];

  // The ScenarioInput control inserts the appropriate markup to get labels & controls
  // hooked into the input section of a scenario page so that it's not repeated in
  // every one.
  var lastError = "";
  var lastStatus = "";
  var ScenarioInput = WinJS.Class.define(
      function (element, options) {
          element.winControl = this;
          this.element = element;

          new WinJS.Utilities.QueryCollection(element)
                      .setAttribute("role", "main")
                      .setAttribute("aria-labelledby", "inputLabel");
          element.id = "input";

          this.addInputLabel(element);
          this.addDetailsElement(element);
          this.addScenariosPicker(element);
      }, {
          addInputLabel: function (element) {
              var label = document.createElement("h2");
              label.textContent = "Input";
              label.id = "inputLabel";
              element.parentNode.insertBefore(label, element);
          },
          addScenariosPicker: function (parentElement) {
              var scenarioList = document.createElement("div");
              scenarioList.id = "scenarios";
              var control = new ScenarioSelect(scenarioList);
              parentElement.insertBefore(scenarioList, parentElement.childNodes[0]);
          },
          addDetailsElement: function (sourceElement) {
              var detailsDiv = this._createDetailsDiv();
              while (sourceElement.childNodes.length > 0) {
                  detailsDiv.appendChild(sourceElement.removeChild(sourceElement.childNodes[0]));
              }
              sourceElement.appendChild(detailsDiv);
          },
          _createDetailsDiv: function () {
              var detailsDiv = document.createElement("div");

              new WinJS.Utilities.QueryCollection(detailsDiv)
                          .addClass("details")
                          .setAttribute("role", "region")
                          .setAttribute("aria-labelledby", "descLabel")
                          .setAttribute("aria-live", "assertive");

              var label = document.createElement("h3");
              label.textContent = "Description";
              label.id = "descLabel";

              detailsDiv.appendChild(label);
              return detailsDiv;
          },
      }
  );

  // The ScenarioOutput control inserts the appropriate markup to get labels & controls
  // hooked into the output section of a scenario page so that it's not repeated in
  // every one.
  var ScenarioOutput = WinJS.Class.define(
      function (element, options) {
          element.winControl = this;
          this.element = element;
          new WinJS.Utilities.QueryCollection(element)
                      .setAttribute("role", "region")
                      .setAttribute("aria-labelledby", "outputLabel")
                      .setAttribute("aria-live", "assertive");
          element.id = "output";

          this._addOutputLabel(element);
          this._addStatusOutput(element);
      }, {
          _addOutputLabel: function (element) {
              var label = document.createElement("h2");
              label.id = "outputLabel";
              label.textContent = "Output";
              element.parentNode.insertBefore(label, element);
          },
          _addStatusOutput: function (element) {
              var statusDiv = document.createElement("div");
              statusDiv.id = "statusMessage";
              statusDiv.setAttribute("role", "textbox");
              element.insertBefore(statusDiv, element.childNodes[0]);
          }
      }
  );

  // Sample set of contacts to pick from
  var sampleContacts = [
      {
          displayName: "David Jaffe",
          firstName: "David",
          lastName: "Jaffe",
          personalEmail: "david@contoso.com",
          workEmail: "david@cpandl.com",
          workPhone: "",
          homePhone: "248-555-0150",
          mobilePhone: "",
          id: "761cb6fb-0270-451e-8725-bb575eeb24d5"
      },

      {
          displayName: "Kim Abercrombie",
          firstName: "Kim",
          lastName: "Abercrombie",
          personalEmail: "kim@contoso.com",
          workEmail: "kim@adatum.com",
          homePhone: "444 555-0001",
          workPhone: "245 555-0123",
          mobilePhone: "921 555-0187",
          id: "49b0652e-8f39-48c5-853b-e5e94e6b8a11"
      },

      {
          displayName: "Jeff Phillips",
          firstName: "Jeff",
          lastName: "Phillips",
          personalEmail: "jeff@contoso.com",
          workEmail: "jeff@fabrikam.com",
          homePhone: "987-555-0199",
          workPhone: "",
          mobilePhone: "543-555-0111",
          id: "864abfb4-8998-4355-8236-8d69e47ec832"
      },

      {
          displayName: "Arlene Huff",
          firstName: "Arlene",
          lastName: "Huff",
          personalEmail: "arlene@contoso.com",
          workEmail: "",
          homePhone: "",
          workPhone: "",
          mobilePhone: "234-555-0156",
          id: "27347af8-0e92-45b8-b14c-dd70fcd3b4a6"
      },

      {
          displayName: "Miles Reid",
          firstName: "Miles",
          lastName: "Reid",
          personalEmail: "miles@contoso.com",
          workEmail: "miles@proseware.com",
          homePhone: "",
          workPhone: "",
          mobilePhone: "",
          id: "e3d24a99-0e29-41af-9add-18f5e3cfc518"
      },
  ];

  WinJS.Namespace.define("SdkSample", {
      sampleTitle: sampleTitle,
      scenarios: new WinJS.Binding.List(scenarios),
      sampleContacts: sampleContacts
  });
})();