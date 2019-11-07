<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JSASPNETCreateULDynamically._Default" %>

<!DOCTYPE html>
 
<head id="Head1" runat="server">
    <title></title>
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <style type="text/css">
        .btn {
            display:none;
             height:15px;
            font-size:0.5em;
        }
 
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="dynamicUL">
    <input type="button" value="Create a new UL" onclick="createUL(event)"/>
    </div>
    </form>
    <script type="text/javascript">
 
        var tree = [];
        var clientID = 0;
 
        //Create a UL element and attach a Li Element in order to let this UL viewable.
        function createUL(event) {
            var ul = jQuery('<ul>', { text: ">" });
            ul.append(jQuery('<button>', { text: 'Add Li', class: 'btn', onclick: 'addLi(event);return false;' }));
            ul.append(jQuery('<button>', { text: 'Add sub ul', class: 'btn', onclick: 'createUL(event);return false;' }));
            ul.append(jQuery('<button>', { text: 'Delete ul', class: 'btn', onclick: 'deleteUL(event);return false;' }));
 
            ul.append(newLi());
            $(ul).hover(function () {
                $(ul).children("button").show();
            },
            function () {
                { $(ul).children("button").hide(); }
            }
            );
            $(event.target).parent().append(ul);
        }
 
        //return a new li element with many event
        function newLi(liValue) {
            var li;
            if (liValue) {
                li = jQuery(
                    '<li>',
                   {
                       text: liValue,
                       id: clientID++
                   });
            }
            else {
                li = jQuery('<li>',
                   {
                       text: 'new value',
                       id: clientID++
                   });
            }
 
            $(li).click(function () {
                var div = jQuery('<div>');
                var textbox = jQuery("<input type='text'>");
                $(textbox).val(li.text());
                $(textbox).attr("id", clientID++);
                $(textbox).css("Width", div.css("Width"));
                $(textbox).keypress(function (e) {
                    if (e.charCode == 13 || e.keyCode == 13) {
                        if ($(textbox).val().length == 0)
                            deleteLi(textbox);
                        else
                            textbox.replaceWith(newLi($(textbox).val()));
                        return false;
                    }
                });
 
                $(textbox).mouseleave(function () {
                    $(document).mousedown(function (event) {
                        if ($(event.target).attr("id") != $(textbox).attr("id")) {
                            if ($(textbox).val().length == 0)
                                deleteLi(textbox);
                            else
                                textbox.replaceWith(newLi($(textbox).val()));
                        }
                    });
                });
 
 
 
                $(div).append(textbox);
                $(this).replaceWith(div).val($(this).text());
 
            });
 
            return li;
        }
 
        //Delete li 
        //target is a textbox
        function deleteLi(target) {
            var ul = $(target).parent().parent();
            $(target).parent().remove();
            if ($(ul).find($('li')).length == 0) {
                ul.remove();
            }
        }
 
        function addLi(event) {
            $(event.target).parent().append(newLi());
        }
 
        function deleteUL(event) {
            $(event.target).parent().remove();
        }
 
 
        function saveTree() {
            $("#dynamicUL").children("ul").each(function () {
                var node = { NodeType: "ul", Nodes: [] }
                tree.push(node);
                fillNode(this, node.Nodes);
            });
 
            $("#hfldJson").val((JSON.stringify(tree)));
            //clean the tree
            tree = [];
        }
        function fillNode(currentElement, currentTreeNode) {
            $(currentElement).children("ul,li").each(function () {
                if (this.localName == "ul") {
                    var node = { Nodetype: "ul", Nodes: [] }
                    currentTreeNode.push(node);
                    fillNode(this, node.Nodes);
                }
                else if (this.localName == "li") {
                    currentTreeNode.push({ NodeType: "li", Value: $(this).text() });
                }
            });
        }
    </script>
</body>
</html>

