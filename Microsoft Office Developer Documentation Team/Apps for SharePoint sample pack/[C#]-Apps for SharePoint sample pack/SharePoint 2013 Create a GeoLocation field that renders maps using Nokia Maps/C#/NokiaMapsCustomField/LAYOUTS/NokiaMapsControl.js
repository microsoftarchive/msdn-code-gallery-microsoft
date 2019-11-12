var g_appId="NOKIA_MAPS_DEVELOPER_KEY_APP_ID";
var g_authToken="NOKIA_MAPS_DEVELOPER_KEY_AUTHENTICATION_TOKEN";

function $_global_nokiamapscontrol() {
    (function() {
        if (typeof NokiaMapsControlTemplate == "object") {
            return;
        }
        window.NokiaMapsControlTemplate = (function() {
            return {
                NokiaMapsControl_Display: function(rCtx) {
                    if (rCtx == null || rCtx.CurrentFieldValue == null || rCtx.CurrentFieldValue == '')
                        return '';
					
					var _myData = SPClientTemplates.Utility.GetFormContextForCurrentField(rCtx);
                    if (_myData == null || _myData.fieldSchema == null)
                        return '';
					_myData.registerInitCallback(_myData.fieldName, InitControl);
					
					var nokiaMapRedirectControl;
					var _inputId_nokiaMapRedirectControl = _myData.fieldName + '_' + _myData.fieldSchema.Id + '_$nokiaMapField';;
						
					var fldvalue = NokiaMapsControlTemplate.ParseGeolocationValue(rCtx.CurrentFieldValue);
					var nokiaStaticMapUrl = NokiaMapsControlTemplate.GetNokiaStaticMapUrl(fldvalue, 400, 300);
					
					var result = '<div>';
					result += NokiaMapsControlTemplate.GetRenderableFieldValue(fldvalue);
					result += '<BR />';
					result += '<a id="' + STSHtmlEncode(_inputId_nokiaMapRedirectControl) + '" href="javascript:">';
					result += '<img src="' + nokiaStaticMapUrl + '" alt="Nokia Maps" />'
					result += '</a>';
					result += '</div>';
					
					return result;
					
					function RedirectToNokiaMaps() {						
						var nokiaMapStaticUrl = document.getElementById(_inputId_nokiaMapRedirectControl).childNodes[0].src;
						var url = nokiaMapStaticUrl.replace("&nord", "");
						window.open(url);
					}
					function InitControl() {
						nokiaMapRedirectControl = document.getElementById(_inputId_nokiaMapRedirectControl);
						if (nokiaMapRedirectControl != null)
                            AddEvtHandler(nokiaMapRedirectControl, "onclick", RedirectToNokiaMaps);
					}
                },	
				GetNokiaStaticMapUrl: function(fldvalue, width, height) {
					var nokiaStaticMapUrl = 'http://m.nok.it/?app_id=' + g_appId + '&token=' + g_authToken;
					nokiaStaticMapUrl += '&w=' + width + '&h=' + height + '';
					nokiaStaticMapUrl += '&z=16';
					nokiaStaticMapUrl += '&c=' + fldvalue.latitude + ',' + fldvalue.longitude;
					nokiaStaticMapUrl += '&nord';
					
					return nokiaStaticMapUrl
				},
				RedirectToNokiaMaps: function(nokiaMapStaticUrl) {
					var url = nokiaMapStaticUrl.replace("&nord", "");
					window.open(url);
				},
				ParseGeolocationValue: function(fieldValue) {
                    var spatialtype = "POINT";
                    var space = ' ';
                    var openingBracket = '(';
                    var closingBracket = ')';
                    var point = new Object();

                    point.longitude = null;
                    point.latitude = null;
                    point.altitude = null;
                    point.measure = null;
                    if (fieldValue == null || fieldValue == '')
                        return null;
                    var valueIndex = 0;
                    var valueLength = fieldValue.length;
                    var subStr;
                    var argIndex = 0;
                    var index = fieldValue.indexOf(openingBracket, valueIndex);

                    if (index <= valueIndex) {
                        return null;
                    }
                    var headEnd = index;

                    if (fieldValue.charCodeAt(index - 1) == space.charCodeAt(0)) {
                        headEnd--;
                    }
                    subStr = fieldValue.substr(valueIndex, headEnd - valueIndex);
                    if (spatialtype.toLowerCase() != subStr.toLowerCase()) {
                        return null;
                    }
                    valueIndex = index + 1;
                    while (valueIndex < valueLength) {
                        index = fieldValue.indexOf(space, valueIndex);
                        if (index <= valueIndex) {
                            index = fieldValue.indexOf(closingBracket, valueIndex);
                        }
                        if (index <= valueIndex) {
                            return null;
                        }
                        subStr = fieldValue.substr(valueIndex, index - valueIndex);
                        if (argIndex == 0) {
                            point.longitude = parseFloat(subStr);
                        }
                        else if (argIndex == 1) {
                            point.latitude = parseFloat(subStr);
                        }
                        else if (argIndex == 2) {
                            point.altitude = parseFloat(subStr);
                        }
                        else if (argIndex == 3) {
                            point.measure = parseFloat(subStr);
                        }
                        argIndex++;
                        valueIndex = index + 1;
                    }
                    if (argIndex < 2) {
                        return null;
                    }
                    return point;
                },
                BuildGeolocationValue: function(latitude, longitude) {
					var geolocationValue = 'Point (' + longitude + ' ' + latitude + ')';
					return geolocationValue;
				},
				GetRenderableFieldValue: function(fieldValue) {
					var fldValue = 'Longitude: ' + fieldValue.longitude + ', Latitude: ' + fieldValue.latitude;
					return fldValue;
				},
				NokiaMapsControl_Edit: function(rCtx) {
                    if (rCtx == null)
                        return '';
                    
					var _myData = SPClientTemplates.Utility.GetFormContextForCurrentField(rCtx);
                    if (_myData == null || _myData.fieldSchema == null)
                        return '';					
					
					// Download Nokia JS.
					downloadJS('http://api.maps.nokia.com/2.2.0/jsl.js');
					
					_myData.registerInitCallback(_myData.fieldName, InitControl);
					
					var fldvalue = NokiaMapsControlTemplate.ParseGeolocationValue(rCtx.CurrentFieldValue);
					var controlIdHeader = _myData.fieldName + '_' + _myData.fieldSchema.Id + '_$';
					var mapDiv = '';
					var searchControl;
					var nokiaMapField;
					var _inputId_nokiaMapField = controlIdHeader + 'nokiaMapField';
					var _inputId_locationDisplayControl = controlIdHeader + 'locationDisplayControl';
					var _inputId_nativeGeolocationValue = controlIdHeader + 'nativeGeolocationValue';
					var nokiaStaticMapUrl = '';					
					
					mapDiv += '<div>';
					mapDiv += '<H3><label id="' + _inputId_locationDisplayControl + '">';
					if (fldvalue != null) {
						mapDiv += NokiaMapsControlTemplate.GetRenderableFieldValue(fldvalue);
                    }
					mapDiv += '</H3></label>';
					mapDiv += '<label id="' + _inputId_nativeGeolocationValue + '" style="visibility: hidden;">';
					if (fldvalue != null) {
						mapDiv += 'Point(' + fldvalue.longitude + ' ' + fldvalue.latitude +')';
                    }
					mapDiv += '</label>';
					
					mapDiv += '<a id="' + STSHtmlEncode(_inputId_nokiaMapField) + '" href="javascript:">';
					mapDiv += '<img alt="Loading..." src="';
					
					if (fldvalue != null) {
						nokiaStaticMapUrl = NokiaMapsControlTemplate.GetNokiaStaticMapUrl(fldvalue, 400, 300);						
						mapDiv += nokiaStaticMapUrl;
					}
					mapDiv += '" />';
					mapDiv += '</a>';
					mapDiv += '</div>';
					
					var _inputId_address = controlIdHeader + "address";
					var _inputId_searchButton = controlIdHeader + "searchButton";
					
					var result = '<div id="mainDiv">';
					result += '<div id="inputControls">';
					result += '<div id="searchControls">';
					result += '<table>';
					result += '<tr>';
					result += '<td width=100%>';
					result += '<input type="text" id="' + _inputId_address + '" value="" style="width: 100%;"/>';
					result += '</td>';
					result += '<td style="width: 100%; text-align: right;">';
					result += '<input type="button" id="' + _inputId_searchButton + '" value="Search" style="width: 70%;" />';
					result += '</td>';
					result += '</tr>';
					result += '</table>';					
					result += '</div>';
					result += '<div id="mapControls" width=50px />';
					result += mapDiv;					
					result += '</div>';
					result += '</div>';
					
					function downloadJS(jsFile) {
						var thescript = document.createElement('script');
						thescript.setAttribute('type','text/javascript');
						thescript.setAttribute('charset','UTF-8');
						thescript.setAttribute('src',jsFile);
						document.getElementsByTagName('head')[0].appendChild(thescript);
					}
					function RedirectToNokiaMaps() {						
						var nokiaMapStaticUrl = document.getElementById(_inputId_nokiaMapField).childNodes[0].src;
						var url = nokiaMapStaticUrl.replace("&nord", "");
						window.open(url);
					}
					function InitControl() {
						nokiaMapField = document.getElementById(_inputId_nokiaMapField);
						if (nokiaMapField != null) {
                            AddEvtHandler(nokiaMapField, "onclick", RedirectToNokiaMaps);
							
							var fldValue = document.getElementById(_inputId_nativeGeolocationValue).textContent;
							if(typeof fldValue == "undefined" || fldValue == '')
							{
								nokiaMapField.setAttribute('style', 'visibility: hidden;');
							}
						}
							
						searchControl = document.getElementById(_inputId_searchButton);
						if (searchControl != null)
                            AddEvtHandler(searchControl, "onclick", GeocodeAddress);
						
					}
					function GeocodeAddress() {
						if(typeof nokia == "undefined" || typeof nokia.places == "undefined")
						{
							alert('Clear cache. Nokia JS downloaded partially!');
							return;
						}
						
						var addressControl = document.getElementById(_inputId_address);
						if (addressControl == null)
							return;
							
						var address = addressControl.value;	
						
						nokia.Settings.set( "appId", g_appId);
						nokia.Settings.set( "authenticationToken", g_authToken);
						nokia.places.search.manager.geoCode({       searchTerm :  address,			 onComplete:  onGeocodeComplete	  }); 	  
												 
						function onGeocodeComplete(data, requestStatus , requestId) { 		
							if (requestStatus == "OK") 
							{   				    
								var point = new Object();
								point.latitude = data.location.position.latitude;
								point.longitude = data.location.position.longitude;
								
								UpdateGeolocationValue(point.latitude, point.longitude);
							} else if(requestStatus == "ERROR") 
							{        
								alert("GEOCODE FAILED.");
							}								
						}
					}
					function UpdateGeolocationValue(latitude, longitude) {
						//Update native value
						document.getElementById(_inputId_nativeGeolocationValue).textContent = NokiaMapsControlTemplate.BuildGeolocationValue(latitude, longitude);
						
						//update display value
						var point = new Object();
						point.latitude = latitude;
						point.longitude = longitude;						
						document.getElementById(_inputId_locationDisplayControl).textContent = NokiaMapsControlTemplate.GetRenderableFieldValue(point);
						
						//Update Map control.
						var nokiaStaticMapUrl = NokiaMapsControlTemplate.GetNokiaStaticMapUrl(point, 400, 300);
						document.getElementById(_inputId_nokiaMapField).childNodes[0].src = nokiaStaticMapUrl;
						nokiaMapField.setAttribute('style', 'visibility: none;');
					}
					
					_myData.registerGetValueCallback(_myData.fieldName, function() {
						var newValue = document.getElementById(_inputId_nativeGeolocationValue).textContent;
						if(newValue == '')
							return '';
							
						var newFldValue = NokiaMapsControlTemplate.ParseGeolocationValue(newValue);
						return "Point(" + String(newFldValue.longitude) + " " + String(newFldValue.latitude) + ")";
                    });
					
					return result;
                },
                NokiaMapsControl_View: function(inCtx, field, listItem, listSchema) {
					if (field.XSLRender == '1') {
                        return listItem[field.Name].toString();
                    }
                    else {
                        var fldvalue = NokiaMapsControlTemplate.ParseGeolocationValue(listItem[field.Name]);
                        var ret = [];

                        if (fldvalue != null) {
                            ret.push("<a class=\"js-locationfield-callout\" href=\"javascript:void(0)\" liid=\"");
                            ret.push(GenerateIID(inCtx));
                            ret.push("\" fld=\"");
                            ret.push(field.Name);
                            ret.push("\" ><img title=\"");
                            ret.push(STSHtmlEncode(Strings.STS.L_Clippy_Tooltip));
                            ret.push("\"border=0 src=\"" + "/_layouts/15/images/callout-target.png");
                            ret.push("\"/></a>");
                        }
                        return ret.join('');
                    }
                },
                SetupMappyHoverHandlers: function(inCtx) {
                    EnsureScriptFunc("callout.js", "Callout", function() {
                        EnsureScriptFunc("core.js", "GetListItemByIID", function() {
                            EnsureScriptFunc("mquery.js", "m$", function() {
                                ((m$('.js-locationfield-callout')).not(".js-locationfield-calloutInitialized")).forEach(function(e) {
                                    var listItemID = e.getAttribute("liid");
                                    var fieldName = e.getAttribute("fld");
                                    var calloutTitle = '';
                                    var calloutContent = [];
                                    var listItem = GetListItemByIID(listItemID);
                                    var values = NokiaMapsControlTemplate.ParseGeolocationValue(listItem[fieldName]);
									var width=300;
									var nokiaMapStaticUrl = NokiaMapsControlTemplate.GetNokiaStaticMapUrl(values, width, 250);

                                    calloutContent.push("<div><div class='ms-positionRelative' id='loc_mapcontainer_");
                                    calloutContent.push(listItemID);
                                    calloutContent.push("_");
                                    calloutContent.push(fieldName);
                                    //calloutContent.push("' ></div></div>");
									calloutContent.push("' ></div>");
									calloutContent.push("<div>");
									calloutContent.push('<img src="' + nokiaMapStaticUrl + '" alt="Nokia Maps" />');
									calloutContent.push("</div>");
									calloutContent.push("</div>");
                                    
                                    var callout = CalloutManager.createNew({
                                        launchPoint: e,
                                        openOptions: {
                                            closeCalloutOnBlur: true,
                                            event: "click",
                                            showCloseButton: true
                                        },
                                        ID: listItemID + "_" + fieldName,
                                        title: calloutTitle,
                                        content: calloutContent.join(''),                                        
                                        contentWidth: width+40										
                                    });
                                    
									callout.addAction(new CalloutAction({
                                        text: "Browse on Nokia Maps",
                                        onClickCallback: function() {
                                            NokiaMapsControlTemplate.RedirectToNokiaMaps(nokiaMapStaticUrl);
                                        }
                                    }));
									
                                    (m$(e)).addClass("js-locationfield-calloutInitialized");
                                });
                            });
                        });
                    });
                },
                NokiaMapsControl_PreRender: function(inCtx) {
					
                },
                NokiaMapsControl_PostRender: function(inCtx) {
					if (ctx != null && ctx.BaseViewID != null && inCtx != null && inCtx.BaseViewID != null) {
                        if (inCtx.BaseViewID == ctx.BaseViewID) {
                            NokiaMapsControlTemplate.SetupMappyHoverHandlers(inCtx);                            
                        }
                    }
                }
            };
        })();
		
        function _registerNokiaMapsControlTemplate() {
            var nokiaMapsControlContext = {};

            nokiaMapsControlContext.Templates = {};
            nokiaMapsControlContext.OnPreRender = NokiaMapsControlTemplate.NokiaMapsControl_PreRender;
            nokiaMapsControlContext.OnPostRender = NokiaMapsControlTemplate.NokiaMapsControl_PostRender;
            nokiaMapsControlContext.Templates.Fields = {
                'NokiaMapsCustomField': {
                    'View': NokiaMapsControlTemplate.NokiaMapsControl_View,
                    'DisplayForm': NokiaMapsControlTemplate.NokiaMapsControl_Display,
                    'EditForm': NokiaMapsControlTemplate.NokiaMapsControl_Edit,
                    'NewForm': NokiaMapsControlTemplate.NokiaMapsControl_Edit
                }
            };
            SPClientTemplates.TemplateManager.RegisterTemplateOverrides(nokiaMapsControlContext);
        }
        ExecuteOrDelayUntilScriptLoaded(_registerNokiaMapsControlTemplate, 'clienttemplates.js');
    })();
}

$_global_nokiamapscontrol();