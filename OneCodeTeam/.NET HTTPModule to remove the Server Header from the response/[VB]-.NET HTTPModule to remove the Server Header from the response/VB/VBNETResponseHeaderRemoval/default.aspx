<%
For Each var as String in Request.ServerVariables
  Response.Write(var & " " & Request(var) & "<br>")
Next
%>