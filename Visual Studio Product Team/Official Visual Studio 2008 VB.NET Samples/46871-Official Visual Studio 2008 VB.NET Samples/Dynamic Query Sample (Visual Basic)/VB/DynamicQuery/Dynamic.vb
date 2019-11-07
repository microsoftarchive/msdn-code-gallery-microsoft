Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Threading
Imports System.Runtime.CompilerServices

Namespace System.Linq.Dynamic
    Public Module DynamicQueryable

        <Extension()> _
        Public Function Where(Of T)(ByVal source As IQueryable(Of T), ByVal predicate As String, ByVal ParamArray values() As Object) As IQueryable(Of T)
            Return DirectCast(Where(DirectCast(source, IQueryable), predicate, values), IQueryable(Of T))
        End Function

        <Extension()> _
        Public Function Where(ByVal source As IQueryable, ByVal predicate As String, ByVal ParamArray values() As Object) As IQueryable
            If source Is Nothing Then Throw New ArgumentNullException("source")
            If predicate Is Nothing Then Throw New ArgumentNullException("predicate")
            Dim lambda As LambdaExpression = DynamicExpression.ParseLambda(source.ElementType, GetType(Boolean), predicate, values)
            Return source.Provider.CreateQuery( _
                Expression.Call( _
                    GetType(Queryable), "Where", _
                    New Type() {source.ElementType}, _
                    source.Expression, Expression.Quote(lambda)))
        End Function

        <Extension()> _
        Public Function [Select](ByVal source As IQueryable, ByVal selector As String, ByVal ParamArray values() As Object) As IQueryable
            If source Is Nothing Then Throw New ArgumentNullException("source")
            If selector Is Nothing Then Throw New ArgumentNullException("selector")
            Dim lambda As LambdaExpression = DynamicExpression.ParseLambda(source.ElementType, Nothing, selector, values)
            Return source.Provider.CreateQuery( _
                Expression.Call( _
                    GetType(Queryable), "Select", _
                    New Type() {source.ElementType, lambda.Body.Type}, _
                    source.Expression, Expression.Quote(lambda)))
        End Function

        <Extension()> _
        Public Function OrderBy(Of T)(ByVal source As IQueryable(Of T), ByVal ordering As String, ByVal ParamArray values() As Object) As IQueryable(Of T)
            Return DirectCast(OrderBy(DirectCast(source, IQueryable), ordering, values), IQueryable(Of T))
        End Function

        <Extension()> _
        Public Function OrderBy(ByVal source As IQueryable, ByVal ordering As String, ByVal ParamArray values() As Object) As IQueryable
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            If (ordering Is Nothing) Then Throw New ArgumentNullException("ordering")
            Dim parameters = New ParameterExpression() { _
                Expression.Parameter(source.ElementType, "")}
            Dim parser As New ExpressionParser(parameters, ordering, values)
            Dim orderings As IEnumerable(Of DynamicOrdering) = parser.ParseOrdering()
            Dim queryExpr As Expression = source.Expression
            Dim methodAsc = "OrderBy"
            Dim methodDesc = "OrderByDescending"
            For Each o As DynamicOrdering In orderings
                queryExpr = Expression.Call( _
                    GetType(Queryable), If(o.Ascending, methodAsc, methodDesc), _
                    New Type() {source.ElementType, o.Selector.Type}, _
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)))
                methodAsc = "ThenBy"
                methodDesc = "ThenByDescending"
            Next o
            Return source.Provider.CreateQuery(queryExpr)
        End Function

        <Extension()> _
        Public Function Take(ByVal source As IQueryable, ByVal count As Integer) As IQueryable
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            Return source.Provider.CreateQuery( _
                Expression.Call( _
                    GetType(Queryable), "Take", _
                    New Type() {source.ElementType}, _
                    source.Expression, Expression.Constant(count)))
        End Function

        <Extension()> _
        Public Function Skip(ByVal source As IQueryable, ByVal count As Integer) As IQueryable
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            Return source.Provider.CreateQuery( _
                Expression.Call( _
                    GetType(Queryable), "Skip", _
                    New Type() {source.ElementType}, _
                    source.Expression, Expression.Constant(count)))
        End Function

        <Extension()> _
        Public Function GroupBy(ByVal source As IQueryable, ByVal keySelector As String, ByVal elementSelector As String, ByVal ParamArray values() As Object) As IQueryable
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            If (keySelector Is Nothing) Then Throw New ArgumentNullException("keySelector")
            If (elementSelector Is Nothing) Then Throw New ArgumentNullException("elementSelector")
            Dim keyLambda As LambdaExpression = DynamicExpression.ParseLambda(source.ElementType, Nothing, keySelector, values)
            Dim elementLambda As LambdaExpression = DynamicExpression.ParseLambda(source.ElementType, Nothing, elementSelector, values)
            Return source.Provider.CreateQuery( _
                Expression.Call( _
                    GetType(Queryable), "GroupBy", _
                    New Type() {source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type}, _
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)))
        End Function

        <Extension()> _
        Public Function Any(ByVal source As IQueryable) As Boolean
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            Return CBool(source.Provider.Execute( _
                Expression.Call( _
                    GetType(Queryable), "Any", _
                    New Type() {source.ElementType}, source.Expression)))
        End Function

        <Extension()> _
        Public Function Count(ByVal source As IQueryable) As Integer
            If (source Is Nothing) Then Throw New ArgumentNullException("source")
            Return CInt(source.Provider.Execute( _
                Expression.Call( _
                    GetType(Queryable), "Count", _
                    New Type() {source.ElementType}, source.Expression)))
        End Function
    End Module

    Public MustInherit Class DynamicClass
        Public Overrides Function ToString() As String
            Dim props = Me.GetType().GetProperties(BindingFlags.Instance Or BindingFlags.Public)
            Dim sb As New StringBuilder()
            sb.Append("{")
            For i As Integer = 0 To props.Length - 1
                If (i > 0) Then sb.Append(", ")
                sb.Append(props(i).Name)
                sb.Append("=")
                sb.Append(props(i).GetValue(Me, Nothing))
            Next i

            sb.Append("}")

            Return sb.ToString()
        End Function
    End Class

    Public Class DynamicProperty
        Private _name As String
        Private _type As Type

        Public Sub New(ByVal name As String, ByVal type As Type)
            If (name Is Nothing) Then Throw New ArgumentNullException("name")
            If (type Is Nothing) Then Throw New ArgumentNullException("type")
            Me._name = name
            Me._type = type
        End Sub

        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property Type() As Type
            Get
                Return _type
            End Get
        End Property
    End Class

    Public Module DynamicExpression
        Public Function Parse(ByVal resultType As Type, ByVal expression As String, ByVal ParamArray values() As Object) As Expression
            Dim parser As New ExpressionParser(Nothing, expression, values)
            Return parser.Parse(resultType)
        End Function

        Public Function ParseLambda(ByVal itType As Type, ByVal resultType As Type, ByVal expressionStr As String, ByVal ParamArray values() As Object) As LambdaExpression
            Return ParseLambda(New ParameterExpression() {Expression.Parameter(itType, "")}, resultType, expressionStr, values)
        End Function

        Public Function ParseLambda(ByVal parameters() As ParameterExpression, ByVal resultType As Type, ByVal expressionStr As String, ByVal ParamArray values() As Object) As LambdaExpression
            Dim parser As New ExpressionParser(parameters, expressionStr, values)
            Return Expression.Lambda(parser.Parse(resultType), parameters)
        End Function

        Public Function ParseLambda(Of T, S)(ByVal expression As String, ByVal ParamArray values() As Object) As Expression(Of Func(Of T, S))
            Return DirectCast(ParseLambda(GetType(T), GetType(S), expression, values), Expression(Of Func(Of T, S)))
        End Function

        Public Function CreateClass(ByVal ParamArray properties() As DynamicProperty) As Type
            Return ClassFactory.Instance.GetDynamicClass(properties)
        End Function

        Public Function CreateClass(ByVal properties As IEnumerable(Of DynamicProperty)) As Type
            Return ClassFactory.Instance.GetDynamicClass(properties)
        End Function
    End Module

    Friend Class DynamicOrdering
        Public Selector As Expression
        Public Ascending As Boolean
    End Class

    Friend Class Signature : Implements IEquatable(Of Signature)
        Public properties() As DynamicProperty
        Public hashCode As Integer

        Public Sub New(ByVal properties As IEnumerable(Of DynamicProperty))
            Me.properties = properties.ToArray()
            hashCode = 0
            For Each p As DynamicProperty In Me.properties
                hashCode = hashCode Xor p.Name.GetHashCode() Xor p.Type.GetHashCode()
            Next p
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Return hashCode
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim cast = TryCast(obj, Signature)
            Return If(cast IsNot Nothing, Equals(cast), False)
        End Function

        Public Overloads Function Equals(ByVal other As Signature) As Boolean Implements IEquatable(Of System.Linq.Dynamic.Signature).Equals
            If (properties.Length <> other.properties.Length) Then Return False
            For i As Integer = 0 To properties.Length - 1
                If (properties(i).Name <> other.properties(i).Name OrElse _
                    Not properties(i).Type.Equals(other.properties(i).Type)) Then
                    Return False
                End If
            Next i
            Return True
        End Function
    End Class

    Friend Class ClassFactory
        Public Shared ReadOnly Instance As New ClassFactory()

        Shared Sub New()
            ' Trigger lazy initialization of static fields
        End Sub

        Private [module] As ModuleBuilder
        Private classes As Dictionary(Of Signature, Type)
        Private classCount As Integer
        Private rwLock As ReaderWriterLock

        Private Sub New()
            Dim name As New AssemblyName("DynamicClasses")
            Dim assembly As AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run)
#If ENABLE_LINQ_PARTIAL_TRUST Then
            call new ReflectionPermission(PermissionState.Unrestricted).Assert()
#End If
            Try
                [module] = assembly.DefineDynamicModule("Module")
            Finally
#If ENABLE_LINQ_PARTIAL_TRUST Then
                PermissionSet.RevertAssert()
#End If
            End Try
            classes = New Dictionary(Of Signature, Type)()
            rwLock = New ReaderWriterLock()
        End Sub

        Public Function GetDynamicClass(ByVal properties As IEnumerable(Of DynamicProperty)) As Type
            rwLock.AcquireReaderLock(Timeout.Infinite)

            Try
                Dim signature As New Signature(properties)
                Dim type As Type = Nothing
                If Not classes.TryGetValue(signature, type) Then
                    type = CreateDynamicClass(signature.properties)
                    classes.Add(signature, type)
                End If
                Return type
            Finally
                rwLock.ReleaseReaderLock()
            End Try
        End Function

        Private Function CreateDynamicClass(ByVal properties() As DynamicProperty) As Type
            Dim cookie As LockCookie = rwLock.UpgradeToWriterLock(Timeout.Infinite)
            Try
                Dim typeName = "DynamicClass" & (classCount + 1)
#If ENABLE_LINQ_PARTIAL_TRUST Then
                Call New ReflectionPermission(PermissionState.Unrestricted).Assert()
#End If
                Try
                    Dim tb As TypeBuilder = Me.module.DefineType(typeName, TypeAttributes.Class Or _
                        TypeAttributes.Public, GetType(DynamicClass))
                    Dim fields() As FieldInfo = GenerateProperties(tb, properties)
                    GenerateEquals(tb, fields)
                    GenerateGetHashCode(tb, fields)
                    Dim result As Type = tb.CreateType()
                    classCount += 1
                    Return result
                Finally
#If ENABLE_LINQ_PARTIAL_TRUST Then
                    PermissionSet.RevertAssert()
#End If
                End Try
            Finally
                rwLock.DowngradeFromWriterLock(cookie)
            End Try
        End Function

        Private Function GenerateProperties(ByVal tb As TypeBuilder, ByVal properties() As DynamicProperty) As FieldInfo()
            Dim fields(properties.Length - 1) As FieldInfo

            For i As Integer = 0 To properties.Length - 1
                Dim dp As DynamicProperty = properties(i)
                Dim fb As FieldBuilder = tb.DefineField("_" & dp.Name, dp.Type, FieldAttributes.Private)
                Dim pb As PropertyBuilder = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, Nothing)
                Dim mbGet As MethodBuilder = tb.DefineMethod("get_" + dp.Name, _
                    MethodAttributes.Public Or MethodAttributes.SpecialName Or MethodAttributes.HideBySig, _
                    dp.Type, Type.EmptyTypes)
                Dim genGet As ILGenerator = mbGet.GetILGenerator()
                genGet.Emit(OpCodes.Ldarg_0)
                genGet.Emit(OpCodes.Ldfld, fb)
                genGet.Emit(OpCodes.Ret)
                Dim mbSet As MethodBuilder = tb.DefineMethod("set_" & dp.Name, _
                    MethodAttributes.Public Or MethodAttributes.SpecialName Or MethodAttributes.HideBySig, _
                    Nothing, New Type() {dp.Type})
                Dim genSet As ILGenerator = mbSet.GetILGenerator()
                genSet.Emit(OpCodes.Ldarg_0)
                genSet.Emit(OpCodes.Ldarg_1)
                genSet.Emit(OpCodes.Stfld, fb)
                genSet.Emit(OpCodes.Ret)
                pb.SetGetMethod(mbGet)
                pb.SetSetMethod(mbSet)
                fields(i) = fb
            Next i

            Return fields
        End Function

        Private Sub GenerateEquals(ByVal tb As TypeBuilder, ByVal fields As FieldInfo())
            Dim mb As MethodBuilder = tb.DefineMethod("Equals", _
                MethodAttributes.Public Or MethodAttributes.ReuseSlot Or _
                MethodAttributes.Virtual Or MethodAttributes.HideBySig, _
                GetType(Boolean), New Type() {GetType(Object)})
            Dim gen As ILGenerator = mb.GetILGenerator()
            Dim other As LocalBuilder = gen.DeclareLocal(tb)
            Dim [next] As Label = gen.DefineLabel()
            gen.Emit(OpCodes.Ldarg_1)
            gen.Emit(OpCodes.Isinst, tb)
            gen.Emit(OpCodes.Stloc, other)
            gen.Emit(OpCodes.Ldloc, other)
            gen.Emit(OpCodes.Brtrue_S, [next])
            gen.Emit(OpCodes.Ldc_I4_0)
            gen.Emit(OpCodes.Ret)
            gen.MarkLabel([next])
            For Each field As FieldInfo In fields
                Dim ft As Type = field.FieldType
                Dim ct As Type = GetType(EqualityComparer(Of Object)).GetGenericTypeDefinition().MakeGenericType(ft)
                [next] = gen.DefineLabel()
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), Nothing)
                gen.Emit(OpCodes.Ldarg_0)
                gen.Emit(OpCodes.Ldfld, field)
                gen.Emit(OpCodes.Ldloc, other)
                gen.Emit(OpCodes.Ldfld, field)
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", New Type() {ft, ft}), Nothing)
                gen.Emit(OpCodes.Brtrue_S, [next])
                gen.Emit(OpCodes.Ldc_I4_0)
                gen.Emit(OpCodes.Ret)
                gen.MarkLabel([next])
            Next
            gen.Emit(OpCodes.Ldc_I4_1)
            gen.Emit(OpCodes.Ret)
        End Sub

        Private Sub GenerateGetHashCode(ByVal tb As TypeBuilder, ByVal fields As FieldInfo())
            Dim mb As MethodBuilder = tb.DefineMethod("GetHashCode", _
                MethodAttributes.Public Or MethodAttributes.ReuseSlot Or _
                MethodAttributes.Virtual Or MethodAttributes.HideBySig, _
                GetType(Integer), Type.EmptyTypes)
            Dim gen As ILGenerator = mb.GetILGenerator()
            gen.Emit(OpCodes.Ldc_I4_0)
            For Each field As FieldInfo In fields
                Dim ft As Type = field.FieldType
                Dim ct As Type = GetType(EqualityComparer(Of Object)).GetGenericTypeDefinition().MakeGenericType(ft)
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), Nothing)
                gen.Emit(OpCodes.Ldarg_0)
                gen.Emit(OpCodes.Ldfld, field)
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", New Type() {ft}), Nothing)
                gen.Emit(OpCodes.Xor)
            Next
            gen.Emit(OpCodes.Ret)
        End Sub
    End Class

    Public NotInheritable Class ParseException : Inherits Exception
        Private positionValue As Integer

        Public Sub New(ByVal message As String, ByVal position As Integer)
            MyBase.New(message)
            Me.positionValue = position
        End Sub

        Public ReadOnly Property Position() As Integer
            Get
                Return positionValue
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format(Res.ParseExceptionFormat, Message, Position)
        End Function
    End Class

    Class ExpressionParser
        Structure Token
            Public id As TokenId
            Public text As String
            Public pos As Integer
        End Structure

        Enum TokenId
            Unknown
            [End]
            Identifier
            StringLiteral
            IntegerLiteral
            RealLiteral
            Exclamation
            Percent
            Amphersand
            OpenParen
            CloseParen
            Asterisk
            Plus
            Comma
            Minus
            Dot
            Slash
            Colon
            LessThan
            Equal
            GreaterThan
            Question
            OpenBracket
            CloseBracket
            Bar
            ExclamationEqual
            DoubleAmphersand
            LessThanEqual
            LessGreater
            DoubleEqual
            GreaterThanEqual
            DoubleBar
        End Enum

        Interface ILogicalSignatures
            Sub F(ByVal x As Boolean, ByVal y As Boolean)
            Sub F(ByVal x? As Boolean, ByVal y? As Boolean)
        End Interface

        Interface IArithmeticSignatures
            Sub F(ByVal x As Integer, ByVal y As Integer)
            Sub F(ByVal x As UInteger, ByVal y As UInteger)
            Sub F(ByVal x As Long, ByVal y As Long)
            Sub F(ByVal x As ULong, ByVal y As ULong)
            Sub F(ByVal x As Single, ByVal y As Single)
            Sub F(ByVal x As Double, ByVal y As Double)
            Sub F(ByVal x As Decimal, ByVal y As Decimal)
            Sub F(ByVal x? As Integer, ByVal y? As Integer)
            Sub F(ByVal x? As UInteger, ByVal y? As UInteger)
            Sub F(ByVal x? As Long, ByVal y? As Long)
            Sub F(ByVal x? As ULong, ByVal y? As ULong)
            Sub F(ByVal x? As Single, ByVal y? As Single)
            Sub F(ByVal x? As Double, ByVal y? As Double)
            Sub F(ByVal x? As Decimal, ByVal y? As Decimal)
        End Interface

        Interface IRelationalSignatures : Inherits IArithmeticSignatures
            Overloads Sub F(ByVal x As String, ByVal y As String)
            Overloads Sub F(ByVal x As Char, ByVal y As Char)
            Overloads Sub F(ByVal x As DateTime, ByVal y As DateTime)
            Overloads Sub F(ByVal x As TimeSpan, ByVal y As TimeSpan)
            Overloads Sub F(ByVal x? As Char, ByVal y? As Char)
            Overloads Sub F(ByVal x? As DateTime, ByVal y? As DateTime)
            Overloads Sub F(ByVal x? As TimeSpan, ByVal y? As TimeSpan)
        End Interface

        Interface IEqualitySignatures : Inherits IRelationalSignatures
            Overloads Sub F(ByVal x As Boolean, ByVal y As Boolean)
            Overloads Sub F(ByVal x? As Boolean, ByVal y? As Boolean)
        End Interface

        Interface IAddSignatures : Inherits IArithmeticSignatures
            Overloads Sub F(ByVal x As DateTime, ByVal y As TimeSpan)
            Overloads Sub F(ByVal x As TimeSpan, ByVal y As TimeSpan)
            Overloads Sub F(ByVal x? As DateTime, ByVal y? As TimeSpan)
            Overloads Sub F(ByVal x? As TimeSpan, ByVal y? As TimeSpan)
        End Interface

        Interface ISubtractSignatures : Inherits IAddSignatures
            Overloads Sub F(ByVal x As DateTime, ByVal y As DateTime)
            Overloads Sub F(ByVal x? As DateTime, ByVal y? As DateTime)
        End Interface

        Interface INegationSignatures
            Sub F(ByVal x As Integer)
            Sub F(ByVal x As Long)
            Sub F(ByVal x As Single)
            Sub F(ByVal x As Double)
            Sub F(ByVal x As Decimal)
            Sub F(ByVal x As Integer?)
            Sub F(ByVal x As Long?)
            Sub F(ByVal x As Single?)
            Sub F(ByVal x As Double?)
            Sub F(ByVal x As Decimal?)
        End Interface

        Interface INotSignatures
            Sub F(ByVal x As Boolean)
            Sub F(ByVal x? As Boolean)
        End Interface

        Interface IEnumerableSignatures
            Sub Where(ByVal predicate As Boolean)
            Sub Any()
            Sub Any(ByVal predicate As Boolean)
            Sub All(ByVal predicate As Boolean)
            Sub Count()
            Sub Count(ByVal predicate As Boolean)
            Sub Min(ByVal selector As Object)
            Sub Max(ByVal selector As Object)
            Sub Sum(ByVal selector As Integer)
            Sub Sum(ByVal selector? As Integer)
            Sub Sum(ByVal selector As Long)
            Sub Sum(ByVal selector? As Long)
            Sub Sum(ByVal selector As Single)
            Sub Sum(ByVal selector? As Single)
            Sub Sum(ByVal selector As Double)
            Sub Sum(ByVal selector? As Double)
            Sub Sum(ByVal selector As Decimal)
            Sub Sum(ByVal selector? As Decimal)
            Sub Average(ByVal selector As Integer)
            Sub Average(ByVal selector? As Integer)
            Sub Average(ByVal selector As Long)
            Sub Average(ByVal selector? As Long)
            Sub Average(ByVal selector As Single)
            Sub Average(ByVal selector? As Single)
            Sub Average(ByVal selector As Double)
            Sub Average(ByVal selector? As Double)
            Sub Average(ByVal selector As Decimal)
            Sub Average(ByVal selector? As Decimal)
        End Interface

        Shared ReadOnly predefinedTypes As Type() = { _
            GetType(Object), _
            GetType(Boolean), _
            GetType(Char), _
            GetType(String), _
            GetType(SByte), _
            GetType(Byte), _
            GetType(Int16), _
            GetType(UInt16), _
            GetType(Int32), _
            GetType(UInt32), _
            GetType(Int64), _
            GetType(UInt64), _
            GetType(Single), _
            GetType(Double), _
            GetType(Decimal), _
            GetType(DateTime), _
            GetType(TimeSpan), _
            GetType(Guid), _
            GetType(Math), _
            GetType(Convert) _
        }

        Shared ReadOnly trueLiteral As Expression = Expression.Constant(True)
        Shared ReadOnly falseLiteral As Expression = Expression.Constant(False)
        Shared ReadOnly nullLiteral As Expression = Expression.Constant(Nothing)

        Shared ReadOnly keywordIt As String = "it"
        Shared ReadOnly keywordIif As String = "iif"
        Shared ReadOnly keywordNew As String = "new"

        Shared keywords As Dictionary(Of String, Object)

        Dim symbols As Dictionary(Of String, Object)
        Dim externals As IDictionary(Of String, Object)
        Dim literals As Dictionary(Of Expression, String)
        Dim it As ParameterExpression
        Dim text As String
        Dim textPos As Integer
        Dim textLen As Integer
        Dim ch As Char
        Dim tokenVal As Token

        Public Sub New(ByVal parameters As ParameterExpression(), ByVal expression As String, ByVal values As Object())
            If expression Is Nothing Then Throw New ArgumentNullException("expression")
            If keywords Is Nothing Then keywords = CreateKeywords()
            symbols = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            literals = New Dictionary(Of Expression, String)()
            If parameters IsNot Nothing Then ProcessParameters(parameters)
            If values IsNot Nothing Then ProcessValues(values)
            text = expression
            textLen = text.Length
            SetTextPos(0)
            NextToken()
        End Sub

        Sub ProcessParameters(ByVal parameters As ParameterExpression())
            For Each pe As ParameterExpression In parameters
                If Not String.IsNullOrEmpty(pe.Name) Then
                    AddSymbol(pe.Name, pe)
                End If
            Next

            If (parameters.Length = 1 AndAlso String.IsNullOrEmpty(parameters(0).Name)) Then
                it = parameters(0)
            End If
        End Sub

        Sub ProcessValues(ByVal values As Object())
            For i As Integer = 0 To values.Length - 1
                Dim value As Object = values(i)
                If i = values.Length - 1 AndAlso TryCast(value, IDictionary(Of String, Object)) IsNot Nothing Then
                    externals = DirectCast(value, IDictionary(Of String, Object))
                Else
                    AddSymbol("@" & i.ToString(System.Globalization.CultureInfo.InvariantCulture), value)
                End If
            Next
        End Sub

        Sub AddSymbol(ByVal name As String, ByVal value As Object)
            If (symbols.ContainsKey(name)) Then
                Throw ParseError(Res.DuplicateIdentifier, name)
            End If
            symbols.Add(name, value)
        End Sub

        Public Function Parse(ByVal resultType As Type) As Expression
            Dim exprPos As Integer = tokenVal.pos
            Dim expr As Expression = ParseExpression()
            If resultType IsNot Nothing Then
                expr = PromoteExpression(expr, resultType, True)
                If expr Is Nothing Then
                    Throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType))
                End If
            End If
            ValidateToken(TokenId.End, Res.SyntaxError)
            Return expr
        End Function

        Public Function ParseOrdering() As IEnumerable(Of DynamicOrdering)
            Dim orderings As List(Of DynamicOrdering) = New List(Of DynamicOrdering)()
            Do
                Dim expr As Expression = ParseExpression()
                Dim ascending As Boolean = True
                If TokenIdentifierIs("asc") OrElse TokenIdentifierIs("ascending") Then
                    NextToken()
                ElseIf TokenIdentifierIs("desc") OrElse TokenIdentifierIs("descending") Then
                    NextToken()
                    ascending = False
                End If
                orderings.Add(New DynamicOrdering() With {.Selector = expr, .Ascending = ascending})
                If tokenVal.id <> TokenId.Comma Then Exit Do
                NextToken()
            Loop
            ValidateToken(TokenId.End, Res.SyntaxError)
            Return orderings
        End Function
        '#pragma warning restore 0219

        ' ?: operator
        Function ParseExpression() As Expression
            Dim errorPos As Integer = tokenVal.pos
            Dim expr As Expression = ParseLogicalOr()
            If tokenVal.id = TokenId.Question Then
                NextToken()
                Dim expr1 As Expression = ParseExpression()
                ValidateToken(TokenId.Colon, Res.ColonExpected)
                NextToken()
                Dim expr2 As Expression = ParseExpression()
                expr = GenerateConditional(expr, expr1, expr2, errorPos)
            End If
            Return expr
        End Function

        ' ||, or operator
        Function ParseLogicalOr() As Expression
            Dim left As Expression = ParseLogicalAnd()
            Do While tokenVal.id = TokenId.DoubleBar OrElse TokenIdentifierIs("or")
                Dim op As Token = tokenVal
                NextToken()
                Dim right As Expression = ParseLogicalAnd()
                CheckAndPromoteOperands(GetType(ILogicalSignatures), op.text, left, right, op.pos)
                left = Expression.OrElse(left, right)
            Loop
            Return left
        End Function

        ' &&, and operator
        Function ParseLogicalAnd() As Expression
            Dim left As Expression = ParseComparison()
            Do While tokenVal.id = TokenId.DoubleAmphersand OrElse TokenIdentifierIs("and")
                Dim op As Token = tokenVal
                NextToken()
                Dim right As Expression = ParseComparison()
                CheckAndPromoteOperands(GetType(ILogicalSignatures), op.text, left, right, op.pos)
                left = Expression.AndAlso(left, right)
            Loop
            Return left
        End Function

        ' =, ==, !=, <>, >, >=, <, <= operators
        Function ParseComparison() As Expression
            Dim left As Expression = ParseAdditive()
            Do While tokenVal.id = TokenId.Equal OrElse tokenVal.id = TokenId.DoubleEqual OrElse _
                tokenVal.id = TokenId.ExclamationEqual OrElse tokenVal.id = TokenId.LessGreater OrElse _
                tokenVal.id = TokenId.GreaterThan OrElse tokenVal.id = TokenId.GreaterThanEqual OrElse _
                tokenVal.id = TokenId.LessThan OrElse tokenVal.id = TokenId.LessThanEqual
                Dim op As Token = tokenVal
                NextToken()
                Dim right As Expression = ParseAdditive()
                Dim isEquality As Boolean = (op.id = TokenId.Equal OrElse op.id = TokenId.DoubleEqual OrElse _
                    op.id = TokenId.ExclamationEqual OrElse op.id = TokenId.LessGreater)
                If isEquality AndAlso Not left.Type.IsValueType AndAlso Not right.Type.IsValueType Then
                    If Not left.Type.Equals(right.Type) Then
                        If left.Type.IsAssignableFrom(right.Type) Then
                            right = Expression.Convert(right, left.Type)
                        ElseIf right.Type.IsAssignableFrom(left.Type) Then
                            left = Expression.Convert(left, right.Type)
                        Else
                            Throw IncompatibleOperandsError(op.text, left, right, op.pos)
                        End If
                    End If
                ElseIf IsEnumType(left.Type) OrElse IsEnumType(right.Type) Then
                    If Not left.Type.Equals(right.Type) Then
                        Dim e As Expression = PromoteExpression(right, left.Type, True)
                        If e IsNot Nothing Then
                            right = e
                        Else
                            e = PromoteExpression(left, right.Type, True)
                            If e Is Nothing Then
                                Throw IncompatibleOperandsError(op.text, left, right, op.pos)
                            End If
                            left = e
                        End If
                    End If
                Else
                    CheckAndPromoteOperands(If(isEquality, GetType(IEqualitySignatures), GetType(IRelationalSignatures)), _
                        op.text, left, right, op.pos)
                End If
                Select Case op.id
                    Case TokenId.Equal, TokenId.DoubleEqual
                        left = GenerateEqual(left, right)
                    Case TokenId.ExclamationEqual, TokenId.LessGreater
                        left = GenerateNotEqual(left, right)
                    Case TokenId.GreaterThan
                        left = GenerateGreaterThan(left, right)
                    Case TokenId.GreaterThanEqual
                        left = GenerateGreaterThanEqual(left, right)
                    Case TokenId.LessThan
                        left = GenerateLessThan(left, right)
                    Case TokenId.LessThanEqual
                        left = GenerateLessThanEqual(left, right)
                End Select
            Loop
            Return left
        End Function

        ' +, -, & operators
        Function ParseAdditive() As Expression
            Dim left = ParseMultiplicative()
            Do While tokenVal.id = TokenId.Plus OrElse tokenVal.id = TokenId.Minus OrElse _
                tokenVal.id = TokenId.Amphersand
                Dim op = tokenVal
                NextToken()
                Dim right = ParseMultiplicative()
                Select Case op.id
                    Case TokenId.Plus
                        If left.Type.Equals(GetType(String)) OrElse right.Type.Equals(GetType(String)) Then
                            GoTo amphersand
                        End If
                        CheckAndPromoteOperands(GetType(IAddSignatures), op.text, left, right, op.pos)
                        left = GenerateAdd(left, right)
                    Case TokenId.Minus
                        CheckAndPromoteOperands(GetType(ISubtractSignatures), op.text, left, right, op.pos)
                        left = GenerateSubtract(left, right)
                    Case TokenId.Amphersand
amphersand:
                        left = GenerateStringConcat(left, right)
                End Select
            Loop
            Return left
        End Function

        ' *, /, %, mod operators
        Function ParseMultiplicative() As Expression
            Dim left = ParseUnary()
            Do While tokenVal.id = TokenId.Asterisk OrElse tokenVal.id = TokenId.Slash OrElse _
                tokenVal.id = TokenId.Percent OrElse TokenIdentifierIs("mod")
                Dim op = tokenVal
                NextToken()
                Dim right = ParseUnary()
                CheckAndPromoteOperands(GetType(IArithmeticSignatures), op.text, left, right, op.pos)
                Select Case op.id
                    Case TokenId.Asterisk
                        left = Expression.Multiply(left, right)
                    Case TokenId.Slash
                        left = Expression.Divide(left, right)
                    Case TokenId.Percent, TokenId.Identifier
                        left = Expression.Modulo(left, right)
                End Select
            Loop
            Return left
        End Function

        ' -, !, not unary operators
        Function ParseUnary() As Expression
            If tokenVal.id = TokenId.Minus OrElse tokenVal.id = TokenId.Exclamation OrElse _
                TokenIdentifierIs("not") Then

                Dim op = tokenVal
                NextToken()
                If op.id = TokenId.Minus AndAlso (tokenVal.id = TokenId.IntegerLiteral OrElse _
                            tokenVal.id = TokenId.RealLiteral) Then
                    tokenVal.text = "-" & tokenVal.text
                    tokenVal.pos = op.pos
                    Return ParsePrimary()
                End If
                Dim expr = ParseUnary()
                If op.id = TokenId.Minus Then
                    CheckAndPromoteOperand(GetType(INegationSignatures), op.text, expr, op.pos)
                    expr = Expression.Negate(expr)
                Else
                    CheckAndPromoteOperand(GetType(INotSignatures), op.text, expr, op.pos)
                    expr = Expression.Not(expr)
                End If
                Return expr
            End If
            Return ParsePrimary()
        End Function

        Function ParsePrimary() As Expression
            Dim expr = ParsePrimaryStart()
            Do
                If tokenVal.id = TokenId.Dot Then
                    NextToken()
                    expr = ParseMemberAccess(Nothing, expr)
                ElseIf tokenVal.id = TokenId.OpenBracket Then
                    expr = ParseElementAccess(expr)
                Else
                    Exit Do
                End If
            Loop
            Return expr
        End Function

        Function ParsePrimaryStart() As Expression
            Select Case tokenVal.id
                Case TokenId.Identifier
                    Return ParseIdentifier()
                Case TokenId.StringLiteral
                    Return ParseStringLiteral()
                Case TokenId.IntegerLiteral
                    Return ParseIntegerLiteral()
                Case TokenId.RealLiteral
                    Return ParseRealLiteral()
                Case TokenId.OpenParen
                    Return ParseParenExpression()
                Case Else
                    Throw ParseError(Res.ExpressionExpected)
            End Select
        End Function

        Function ParseStringLiteral() As Expression
            ValidateToken(TokenId.StringLiteral)

            Dim quote = tokenVal.text(0)
            Dim s = tokenVal.text.Substring(1, tokenVal.text.Length - 2)
            Dim start = 0

            Do
                Dim i = s.IndexOf(quote, start)
                If i < 0 Then Exit Do
                s = s.Remove(i, 1)
                start = i + 1
            Loop

            If quote = "'" Then
                If s.Length <> 1 Then
                    Throw ParseError(Res.InvalidCharacterLiteral)
                End If
                NextToken()
                Return CreateLiteral(s(0), s)
            End If
            NextToken()
            Return CreateLiteral(s, s)
        End Function

        Function ParseIntegerLiteral() As Expression
            ValidateToken(TokenId.IntegerLiteral)
            Dim text = tokenVal.text
            If text(0) <> "-" Then
                Dim value As ULong = 0
                If Not UInt64.TryParse(text, value) Then
                    Throw ParseError(Res.InvalidIntegerLiteral, text)
                End If

                NextToken()
                If value <= CULng(Int32.MaxValue) Then Return CreateLiteral(CInt(value), text)
                If value <= CULng(UInt32.MaxValue) Then Return CreateLiteral(CUInt(value), text)
                If value <= CULng(Int64.MaxValue) Then Return CreateLiteral(CLng(value), text)
                Return CreateLiteral(value, text)
            Else
                Dim value As Long = 0
                If Not Int64.TryParse(text, value) Then
                    Throw ParseError(Res.InvalidIntegerLiteral, text)
                End If
                NextToken()
                If (value >= Int32.MinValue AndAlso value <= Int32.MaxValue) Then
                    Return CreateLiteral(CInt(value), text)
                End If
                Return CreateLiteral(value, text)
            End If
        End Function

        Function ParseRealLiteral() As Expression
            ValidateToken(TokenId.RealLiteral)
            Dim text = tokenVal.text
            Dim value As Object = Nothing
            Dim last = text(text.Length - 1)
            If last = "f" Or last = "F" Then
                Dim f As Single
                If Single.TryParse(text.Substring(0, text.Length - 1), f) Then value = f

            Else
                Dim d As Double
                If Double.TryParse(text, d) Then value = d
            End If

            If value Is Nothing Then Throw ParseError(Res.InvalidRealLiteral, text)
            NextToken()
            Return CreateLiteral(value, text)
        End Function

        Function CreateLiteral(ByVal value As Object, ByVal text As String) As Expression
            Dim expr = Expression.Constant(value)
            literals.Add(expr, text)
            Return expr
        End Function

        Function ParseParenExpression() As Expression
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected)
            NextToken()
            Dim e = ParseExpression()
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected)
            NextToken()
            Return e
        End Function

        Function ParseIdentifier() As Expression
            ValidateToken(TokenId.Identifier)
            Dim value As Object = Nothing
            If keywords.TryGetValue(tokenVal.text, value) Then
                If TryCast(value, Type) IsNot Nothing Then Return ParseTypeAccess(DirectCast(value, Type))
                If value Is keywordIt Then Return ParseIt()
                If value Is keywordIif Then Return ParseIif()
                If value Is keywordNew Then Return ParseNew()
                NextToken()
                Return DirectCast(value, Expression)
            End If

            If symbols.TryGetValue(tokenVal.text, value) OrElse _
                externals IsNot Nothing AndAlso externals.TryGetValue(tokenVal.text, value) Then
                Dim expr = TryCast(value, Expression)
                If expr Is Nothing Then
                    expr = Expression.Constant(value)
                Else
                    Dim lambda = TryCast(expr, LambdaExpression)
                    If lambda IsNot Nothing Then Return ParseLambdaInvocation(lambda)
                End If
                NextToken()
                Return expr
            End If
            If it IsNot Nothing Then Return ParseMemberAccess(Nothing, it)
            Throw ParseError(Res.UnknownIdentifier, tokenVal.text)
        End Function

        Function ParseIt() As Expression
            If it Is Nothing Then Throw ParseError(Res.NoItInScope)
            NextToken()
            Return it
        End Function

        Function ParseIif() As Expression
            Dim errorPos = tokenVal.pos
            NextToken()
            Dim args As Expression() = ParseArgumentList()
            If args.Length <> 3 Then
                Throw ParseError(errorPos, Res.IifRequiresThreeArgs)
            End If
            Return GenerateConditional(args(0), args(1), args(2), errorPos)
        End Function

        Function GenerateConditional(ByVal test As Expression, ByVal expr1 As Expression, ByVal expr2 As Expression, ByVal errorPos As Integer) As Expression
            If Not test.Type.Equals(GetType(Boolean)) Then
                Throw ParseError(errorPos, Res.FirstExprMustBeBool)
            End If
            If Not expr1.Type.Equals(expr2.Type) Then
                Dim expr1as2 As Expression = If(Not expr2.Equals(nullLiteral), PromoteExpression(expr1, expr2.Type, True), Nothing)
                Dim expr2as1 As Expression = If(Not expr1.Equals(nullLiteral), PromoteExpression(expr2, expr1.Type, True), Nothing)
                If expr1as2 IsNot Nothing And expr2as1 Is Nothing Then
                    expr1 = expr1as2
                ElseIf expr2as1 IsNot Nothing And expr1as2 Is Nothing Then
                    expr2 = expr2as1
                Else
                    Dim type1 = If(Not expr1.Equals(nullLiteral), expr1.Type.Name, "null")
                    Dim type2 = If(Not expr2.Equals(nullLiteral), expr2.Type.Name, "null")
                    If expr1as2 IsNot Nothing And expr2as1 IsNot Nothing Then
                        Throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2)
                    End If
                    Throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2)
                End If
            End If
            Return Expression.Condition(test, expr1, expr2)
        End Function

        Function ParseNew() As Expression
            NextToken()
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected)
            NextToken()
            Dim properties As New List(Of DynamicProperty)()
            Dim expressions As New List(Of Expression)()
            Do
                Dim exprPos = tokenVal.pos
                Dim expr = ParseExpression()
                Dim propName As String
                If TokenIdentifierIs("as") Then
                    NextToken()
                    propName = GetIdentifier()
                    NextToken()
                Else
                    Dim [me] As MemberExpression = TryCast(expr, MemberExpression)
                    If [me] Is Nothing Then Throw ParseError(exprPos, Res.MissingAsClause)
                    propName = [me].Member.Name
                End If
                expressions.Add(expr)
                properties.Add(New DynamicProperty(propName, expr.Type))
                If tokenVal.id <> TokenId.Comma Then Exit Do
                NextToken()
            Loop
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected)
            NextToken()
            Dim type As Type = DynamicExpression.CreateClass(properties)
            Dim bindings(properties.Count - 1) As MemberBinding
            For i As Integer = 0 To bindings.Length - 1
                bindings(i) = Expression.Bind(type.GetProperty(properties(i).Name), expressions(i))
            Next
            Return Expression.MemberInit(Expression.[New](type), bindings)
        End Function

        Function ParseLambdaInvocation(ByVal lambda As LambdaExpression) As Expression
            Dim errorPos = tokenVal.pos
            NextToken()
            Dim args As Expression() = ParseArgumentList()
            Dim method As MethodBase = Nothing
            If FindMethod(lambda.Type, "Invoke", False, args, method) <> 1 Then
                Throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda)
            End If
            Return Expression.Invoke(lambda, args)
        End Function

        Function ParseTypeAccess(ByVal type As Type) As Expression
            Dim errorPos = tokenVal.pos
            NextToken()

            If tokenVal.id = TokenId.Question Then
                If (Not type.IsValueType) OrElse IsNullableType(type) Then
                    Throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type))
                End If
                type = GetType(Nullable(Of Integer)).GetGenericTypeDefinition().MakeGenericType(type)
                NextToken()
            End If
            If tokenVal.id = TokenId.OpenParen Then
                Dim args As Expression() = ParseArgumentList()
                Dim method As MethodBase = Nothing
                Select Case FindBestMethod(type.GetConstructors(), args, method)
                    Case 0
                        If args.Length = 1 Then
                            Return GenerateConversion(args(0), type, errorPos)
                        End If
                        Throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type))
                    Case 1
                        Return Expression.[New](DirectCast(method, ConstructorInfo), args)
                    Case Else
                        Throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type))
                End Select
            End If
            ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected)
            NextToken()
            Return ParseMemberAccess(type, Nothing)
        End Function

        Function GenerateConversion(ByVal expr As Expression, ByVal type As Type, ByVal errorPos As Integer) As Expression
            Dim exprType = expr.Type
            If exprType.Equals(type) Then Return expr
            If exprType.IsValueType AndAlso type.IsValueType Then
                If (IsNullableType(exprType) OrElse IsNullableType(type)) AndAlso _
                    GetNonNullableType(exprType).equals(GetNonNullableType(type)) Then

                    Return Expression.Convert(expr, type)
                End If
                If (IsNumericType(exprType) OrElse IsEnumType(exprType)) AndAlso _
                    (IsNumericType(type) OrElse IsEnumType(type)) Then

                    Return Expression.ConvertChecked(expr, type)
                End If
            End If
            If exprType.IsAssignableFrom(type) OrElse type.IsAssignableFrom(exprType) OrElse _
                exprType.IsInterface OrElse type.IsInterface Then
                Return Expression.Convert(expr, type)
            End If
            Throw ParseError(errorPos, Res.CannotConvertValue, _
                GetTypeName(exprType), GetTypeName(type))
        End Function


        Function ParseMemberAccess(ByVal type As Type, ByVal instance As Expression) As Expression
            If instance IsNot Nothing Then type = instance.Type
            Dim errorPos = tokenVal.pos
            Dim id = GetIdentifier()
            NextToken()
            If tokenVal.id = TokenId.OpenParen Then
                If instance IsNot Nothing AndAlso Not type.Equals(GetType(String)) Then
                    Dim enumerableType As Type = FindGenericType(GetType(IEnumerable(Of Object)).GetGenericTypeDefinition(), type)
                    If enumerableType IsNot Nothing Then
                        Dim elementType As Type = enumerableType.GetGenericArguments()(0)
                        Return ParseAggregate(instance, elementType, id, errorPos)
                    End If
                End If
                Dim args As Expression() = ParseArgumentList()
                Dim mb As MethodBase = Nothing
                Select Case FindMethod(type, id, instance Is Nothing, args, mb)
                    Case 0
                        Throw ParseError(errorPos, Res.NoApplicableMethod, id, GetTypeName(type))
                    Case 1
                        Dim method = DirectCast(mb, MethodInfo)
                        If (Not IsPredefinedType(method.DeclaringType)) Then
                            Throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType))
                        End If
                        If method.ReturnType.Equals(GetType(Void)) Then
                            Throw ParseError(errorPos, Res.MethodIsVoid, id, GetTypeName(method.DeclaringType))
                        End If
                        Return Expression.Call(instance, DirectCast(method, MethodInfo), args)
                    Case Else
                        Throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, GetTypeName(type))
                End Select
            Else
                Dim member As MemberInfo = FindPropertyOrField(type, id, instance Is Nothing)
                If member Is Nothing Then
                    Throw ParseError(errorPos, Res.UnknownPropertyOrField, id, GetTypeName(type))
                End If
                Return If(TryCast(member, PropertyInfo) IsNot Nothing, _
                    Expression.Property(instance, DirectCast(member, PropertyInfo)), _
                    Expression.Field(instance, DirectCast(member, FieldInfo)))
            End If
        End Function

        Shared Function FindGenericType(ByVal generic As Type, ByVal type As Type) As Type
            Do While type IsNot Nothing AndAlso Not type.Equals(GetType(Object))
                If type.IsGenericType AndAlso type.GetGenericTypeDefinition().Equals(generic) Then Return type
                If generic.IsInterface Then
                    For Each intfType As Type In type.GetInterfaces()
                        Dim found As Type = FindGenericType(generic, intfType)
                        If found IsNot Nothing Then Return found
                    Next
                End If
                type = type.BaseType
            Loop
            Return Nothing
        End Function

        Function ParseAggregate(ByVal instance As Expression, ByVal elementType As Type, ByVal methodName As String, ByVal errorPos As Integer) As Expression
            Dim outerIt As ParameterExpression = it
            Dim innerIt As ParameterExpression = Expression.Parameter(elementType, "")
            it = innerIt
            Dim args As Expression() = ParseArgumentList()
            it = outerIt
            Dim signature As MethodBase = Nothing
            If FindMethod(GetType(IEnumerableSignatures), methodName, False, args, signature) <> 1 Then
                Throw ParseError(errorPos, Res.NoApplicableAggregate, methodName)
            End If
            Dim typeArgs As Type()
            If signature.Name = "Min" OrElse signature.Name = "Max" Then
                typeArgs = New Type() {elementType, args(0).Type}
            Else
                typeArgs = New Type() {elementType}
            End If

            If args.Length = 0 Then
                args = New Expression() {instance}
            Else
                args = New Expression() {instance, Expression.Lambda(args(0), innerIt)}
            End If
            Return Expression.Call(GetType(Enumerable), signature.Name, typeArgs, args)
        End Function

        Function ParseArgumentList() As Expression()
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected)
            NextToken()
            Dim args As Expression() = If(tokenVal.id <> TokenId.CloseParen, ParseArguments(), New Expression(-1) {})
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected)
            NextToken()
            Return args
        End Function

        Function ParseArguments() As Expression()
            Dim argList As New List(Of Expression)()
            Do
                argList.Add(ParseExpression())
                If tokenVal.id <> TokenId.Comma Then Exit Do
                NextToken()
            Loop
            Return argList.ToArray()
        End Function

        Function ParseElementAccess(ByVal expr As Expression) As Expression
            Dim errorPos As Integer = tokenVal.pos
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected)
            NextToken()
            Dim args As Expression() = ParseArguments()
            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected)
            NextToken()
            If expr.Type.IsArray Then
                If expr.Type.GetArrayRank() <> 1 OrElse args.Length <> 1 Then
                    Throw ParseError(errorPos, Res.CannotIndexMultiDimArray)
                End If
                Dim index As Expression = PromoteExpression(args(0), GetType(Integer), True)
                If index Is Nothing Then
                    Throw ParseError(errorPos, Res.InvalidIndex)
                End If
                Return Expression.ArrayIndex(expr, index)
            Else
                Dim mb As MethodBase = Nothing
                Select Case FindIndexer(expr.Type, args, mb)
                    Case 0
                        Throw ParseError(errorPos, Res.NoApplicableIndexer, GetTypeName(expr.Type))
                    Case 1
                        Return Expression.Call(expr, DirectCast(mb, MethodInfo), args)
                    Case Else
                        Throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, GetTypeName(expr.Type))
                End Select
            End If
        End Function

        Shared Function IsPredefinedType(ByVal type As Type) As Boolean
            For Each t As Type In predefinedTypes
                If t.Equals(type) Then Return True
            Next

            Return False
        End Function

        Shared Function IsNullableType(ByVal type As Type) As Boolean
            Return type.IsGenericType AndAlso type.GetGenericTypeDefinition().Equals(GetType(Nullable(Of Integer)).GetGenericTypeDefinition())
        End Function

        Shared Function GetNonNullableType(ByVal type As Type) As Type
            Return If(IsNullableType(type), type.GetGenericArguments()(0), type)
        End Function

        Shared Function GetTypeName(ByVal type As Type) As String
            Dim baseType = GetNonNullableType(type)
            Dim s = baseType.Name
            If Not type.Equals(baseType) Then s &= "?"
            Return s
        End Function

        Shared Function IsNumericType(ByVal type As Type) As Boolean
            Return GetNumericTypeKind(type) <> 0
        End Function

        Shared Function IsSignedIntegralType(ByVal type As Type) As Boolean
            Return GetNumericTypeKind(type) = 2
        End Function

        Shared Function IsUnsignedIntegralType(ByVal type As Type) As Boolean
            Return GetNumericTypeKind(type) = 3
        End Function

        Shared Function GetNumericTypeKind(ByVal type As Type) As Integer
            type = GetNonNullableType(type)
            If type.IsEnum Then Return 0
            Select Case Type.GetTypeCode(type)
                Case TypeCode.Char, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                    Return 1
                Case TypeCode.SByte, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64
                    Return 2
                Case TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
                    Return 3
                Case Else
                    Return 0
            End Select
        End Function

        Shared Function IsEnumType(ByVal type As Type) As Boolean
            Return GetNonNullableType(type).IsEnum
        End Function

        Sub CheckAndPromoteOperand(ByVal signatures As Type, ByVal opName As String, ByRef expr As Expression, ByVal errorPos As Integer)
            Dim args As Expression() = New Expression() {expr}
            Dim method As MethodBase = Nothing
            If FindMethod(signatures, "F", False, args, method) <> 1 Then
                Throw ParseError(errorPos, Res.IncompatibleOperand, opName, GetTypeName(args(0).Type))
            End If
            expr = args(0)
        End Sub

        Sub CheckAndPromoteOperands(ByVal signatures As Type, ByVal opName As String, ByRef left As Expression, ByRef right As Expression, ByVal errorPos As Integer)
            Dim args As Expression() = New Expression() {left, right}
            Dim method As MethodBase = Nothing
            If FindMethod(signatures, "F", False, args, method) <> 1 Then
                Throw IncompatibleOperandsError(opName, left, right, errorPos)
            End If
            left = args(0)
            right = args(1)
        End Sub

        Function IncompatibleOperandsError(ByVal opName As String, ByVal left As Expression, ByVal right As Expression, ByVal pos As Integer) As Exception
            Return ParseError(pos, Res.IncompatibleOperands, opName, GetTypeName(left.Type), GetTypeName(right.Type))
        End Function

        Function FindPropertyOrField(ByVal type As Type, ByVal memberName As String, ByVal staticAccess As Boolean) As MemberInfo
            Dim flags As BindingFlags = BindingFlags.Public Or BindingFlags.DeclaredOnly Or _
                If(staticAccess, BindingFlags.Static, BindingFlags.Instance)
            For Each t As Type In SelfAndBaseTypes(Type)
                Dim members As MemberInfo() = t.FindMembers(MemberTypes.Property Or MemberTypes.Field, _
                    flags, type.FilterNameIgnoreCase, memberName)
                If members.Length <> 0 Then Return members(0)
            Next
            Return Nothing
        End Function

        Function FindMethod(ByVal type As Type, ByVal methodName As String, ByVal staticAccess As Boolean, ByVal args As Expression(), ByRef method As MethodBase) As Integer
            Dim flags As BindingFlags = BindingFlags.Public Or BindingFlags.DeclaredOnly Or _
                If(staticAccess, BindingFlags.Static, BindingFlags.Instance)
            For Each t As Type In SelfAndBaseTypes(type)
                Dim members As MemberInfo() = t.FindMembers(MemberTypes.Method, _
                    flags, Type.FilterNameIgnoreCase, methodName)
                Dim count As Integer = FindBestMethod(members.Cast(Of MethodBase)(), args, method)
                If count <> 0 Then Return count
            Next
            method = Nothing
            Return 0
        End Function

        Function FindIndexer(ByVal type As Type, ByVal args As Expression(), ByRef method As MethodBase) As Integer
            For Each t As Type In SelfAndBaseTypes(type)
                Dim members As MemberInfo() = t.GetDefaultMembers()
                If members.Length <> 0 Then
                    Dim methods As IEnumerable(Of MethodBase) = members. _
                        OfType(Of PropertyInfo)(). _
                        Select(Function(p) DirectCast(p.GetGetMethod(), MethodBase)). _
                        Where(Function(m) m IsNot Nothing)
                    Dim count As Integer = FindBestMethod(methods, args, method)
                    If count <> 0 Then Return count
                End If
            Next
            method = Nothing
            Return 0
        End Function

        Shared Function SelfAndBaseTypes(ByVal type As Type) As IEnumerable(Of Type)
            If type.IsInterface Then
                Dim types As New List(Of Type)()
                AddInterface(types, type)
                Return types
            End If
            Return SelfAndBaseClasses(type)
        End Function

        Shared Function SelfAndBaseClasses(ByVal type As Type) As IEnumerable(Of Type)
            Dim results As New LinkedList(Of Type)()

            Do While type IsNot Nothing
                results.AddLast(type)
                type = type.BaseType
            Loop

            Return results
        End Function

        Shared Sub AddInterface(ByVal types As List(Of Type), ByVal type As Type)
            If Not types.Contains(type) Then
                types.Add(type)
            End If
            For Each t As Type In type.GetInterfaces()
                AddInterface(types, t)
            Next
        End Sub

        Class MethodData
            Public MethodBase As MethodBase
            Public Parameters As ParameterInfo()
            Public Args As Expression()
        End Class

        Function FindBestMethod(ByVal methods As IEnumerable(Of MethodBase), ByVal args As Expression(), ByRef method As MethodBase) As Integer
            Dim applicable As MethodData() = methods. _
                Select(Function(m) New MethodData With {.MethodBase = m, .Parameters = m.GetParameters()}). _
                Where(Function(m) IsApplicable(m, args)). _
                ToArray()
            If applicable.Length > 1 Then
                applicable = applicable. _
                    Where(Function(m) applicable.All(Function(n) m Is n OrElse IsBetterThan(args, m, n))). _
                    ToArray()
            End If
            If applicable.Length = 1 Then
                Dim md As MethodData = applicable(0)
                For i As Integer = 0 To args.Length - 1
                    args(i) = md.Args(i)
                Next
                method = md.MethodBase
            Else
                method = Nothing
            End If
            Return applicable.Length
        End Function

        Function IsApplicable(ByVal method As MethodData, ByVal args As Expression()) As Boolean
            If method.Parameters.Length <> args.Length Then Return False
            Dim promotedArgs As Expression() = New Expression(args.Length - 1) {}

            For i As Integer = 0 To args.Length - 1
                Dim pi As ParameterInfo = method.Parameters(i)
                If pi.IsOut Then Return False
                Dim promoted As Expression = PromoteExpression(args(i), pi.ParameterType, False)
                If promoted Is Nothing Then Return False
                promotedArgs(i) = promoted
            Next i
            method.Args = promotedArgs

            Return True
        End Function

        Function PromoteExpression(ByVal expr As Expression, ByVal type As Type, ByVal exact As Boolean) As Expression
            If expr.Type.Equals(type) Then Return expr
            If TryCast(expr, ConstantExpression) IsNot Nothing Then
                Dim ce = DirectCast(expr, ConstantExpression)
                If ce.Equals(nullLiteral) Then
                    If Not type.IsValueType OrElse IsNullableType(type) Then
                        Return Expression.Constant(Nothing, type)
                    End If
                Else
                    Dim text As String = Nothing
                    If literals.TryGetValue(ce, text) Then
                        Dim target As Type = GetNonNullableType(type)
                        Dim value As Object = Nothing
                        Select Case Type.GetTypeCode(ce.Type)
                            Case TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64
                                value = ParseNumber(text, target)
                            Case TypeCode.Double
                                If target.Equals(GetType(Decimal)) Then value = ParseNumber(text, target)
                            Case TypeCode.String
                                value = ParseEnum(text, target)
                        End Select
                        If value IsNot Nothing Then Return Expression.Constant(value, type)
                    End If
                End If
            End If

            If IsCompatibleWith(expr.Type, type) Then
                If type.IsValueType OrElse exact Then Return Expression.Convert(expr, type)
                Return expr
            End If
            Return Nothing
        End Function

        Shared Function ParseNumber(ByVal text As String, ByVal type As Type) As Object
            Select Case Type.GetTypeCode(GetNonNullableType(type))
                Case TypeCode.SByte
                    Dim sb As SByte
                    If SByte.TryParse(text, sb) Then Return sb
                Case TypeCode.Byte
                    Dim b As Byte
                    If Byte.TryParse(text, b) Then Return b
                Case TypeCode.Int16
                    Dim s As Short
                    If Short.TryParse(text, s) Then Return s
                Case TypeCode.UInt16
                    Dim us As UShort
                    If UShort.TryParse(text, us) Then Return us
                Case TypeCode.Int32
                    Dim i As Integer
                    If Integer.TryParse(text, i) Then Return i
                Case TypeCode.UInt32
                    Dim ui As UInteger
                    If UInteger.TryParse(text, ui) Then Return ui
                Case TypeCode.Int64
                    Dim l As Long
                    If Long.TryParse(text, l) Then Return l
                Case TypeCode.UInt64
                    Dim ul As ULong
                    If ULong.TryParse(text, ul) Then Return ul
                Case TypeCode.Single
                    Dim f As Single
                    If Single.TryParse(text, f) Then Return f
                Case TypeCode.Double
                    Dim d As Double
                    If Double.TryParse(text, d) Then Return d
                Case TypeCode.Decimal
                    Dim e As Decimal
                    If Decimal.TryParse(text, e) Then Return e
            End Select
            Return Nothing
        End Function

        Shared Function ParseEnum(ByVal name As String, ByVal type As Type) As Object
            If type.IsEnum Then
                Dim memberInfos As MemberInfo() = type.FindMembers(MemberTypes.Field, _
                    BindingFlags.Public Or BindingFlags.DeclaredOnly Or BindingFlags.Static, _
                    Type.FilterNameIgnoreCase, name)
                If memberInfos.Length <> 0 Then Return DirectCast(memberInfos(0), FieldInfo).GetValue(Nothing)
            End If
            Return Nothing
        End Function

        Shared Function IsCompatibleWith(ByVal source As Type, ByVal target As Type) As Boolean
            If source.Equals(target) Then Return True
            If Not target.IsValueType Then Return target.IsAssignableFrom(source)
            Dim st As Type = GetNonNullableType(source)
            Dim tt As Type = GetNonNullableType(target)
            If Not st.Equals(source) AndAlso tt.Equals(target) Then Return False
            Dim sc As TypeCode = If(st.IsEnum, TypeCode.Object, Type.GetTypeCode(st))
            Dim tc As TypeCode = If(tt.IsEnum, TypeCode.Object, Type.GetTypeCode(tt))

            Select Case sc
                Case TypeCode.SByte
                    Select Case tc
                        Case TypeCode.SByte, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.Byte
                    Select Case tc
                        Case TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.Int16
                    Select Case tc
                        Case TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.UInt16
                    Select Case tc
                        Case TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.Int32
                    Select Case tc
                        Case TypeCode.Int32, TypeCode.Int64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.UInt32
                    Select Case tc
                        Case TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.Int64
                    Select Case tc
                        Case TypeCode.Int64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.UInt64
                    Select Case tc
                        Case TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                            Return True
                    End Select
                Case TypeCode.Single
                    Select Case tc
                        Case TypeCode.Single, TypeCode.Double
                            Return True
                    End Select
                Case Else
                    If st.Equals(tt) Then Return True
            End Select
            Return False
        End Function

        Shared Function IsBetterThan(ByVal args As Expression(), ByVal m1 As MethodData, ByVal m2 As MethodData) As Boolean
            Dim better = False
            For i As Integer = 0 To args.Length - 1
                Dim c As Integer = CompareConversions(args(i).Type, _
                    m1.Parameters(i).ParameterType, _
                    m2.Parameters(i).ParameterType)
                If c < 0 Then Return False
                If c > 0 Then better = True
            Next i
            Return better
        End Function

        ' Return 1 if s -> t1 is a better conversion than s -> t2
        ' Return -1 if s -> t2 is a better conversion than s -> t1
        ' Return 0 if neither conversion is better
        Shared Function CompareConversions(ByVal s As Type, ByVal t1 As Type, ByVal t2 As Type) As Integer
            If t1.equals(t2) Then Return 0
            If s.equals(t1) Then Return 1
            If s.equals(t2) Then Return -1
            Dim t1t2 As Boolean = IsCompatibleWith(t1, t2)
            Dim t2t1 As Boolean = IsCompatibleWith(t2, t1)
            If t1t2 AndAlso Not t2t1 Then Return 1
            If t2t1 AndAlso Not t1t2 Then Return -1
            If IsSignedIntegralType(t1) AndAlso IsUnsignedIntegralType(t2) Then Return 1
            If IsSignedIntegralType(t2) AndAlso IsUnsignedIntegralType(t1) Then Return -1
            Return 0
        End Function

        Function GenerateEqual(ByVal left As Expression, ByVal right As Expression) As Expression
            Return Expression.Equal(left, right)
        End Function

        Function GenerateNotEqual(ByVal left As Expression, ByVal right As Expression) As Expression
            Return Expression.NotEqual(left, right)
        End Function

        Function GenerateGreaterThan(ByVal left As Expression, ByVal right As Expression) As Expression
            If left.Type.Equals(GetType(String)) Then
                Return Expression.GreaterThan( _
                    GenerateStaticMethodCall("Compare", left, right), _
                    Expression.Constant(0))
            End If
            Return Expression.GreaterThan(left, right)
        End Function

        Function GenerateGreaterThanEqual(ByVal left As Expression, ByVal right As Expression) As Expression
            If Left.Type.equals(GetType(String)) Then
                Return Expression.GreaterThanOrEqual( _
                    GenerateStaticMethodCall("Compare", left, right), _
                    Expression.Constant(0))
            End If
            Return Expression.GreaterThanOrEqual(left, right)
        End Function

        Function GenerateLessThan(ByVal left As Expression, ByVal right As Expression) As Expression
            If left.Type.Equals(GetType(String)) Then
                Return Expression.LessThan( _
                    GenerateStaticMethodCall("Compare", left, right), _
                    Expression.Constant(0))
            End If
            Return Expression.LessThan(left, right)
        End Function

        Function GenerateLessThanEqual(ByVal left As Expression, ByVal right As Expression) As Expression
            If left.Type.Equals(GetType(String)) Then
                Return Expression.LessThanOrEqual( _
                    GenerateStaticMethodCall("Compare", left, right), _
                    Expression.Constant(0))
            End If
            Return Expression.LessThanOrEqual(left, right)
        End Function

        Function GenerateAdd(ByVal left As Expression, ByVal right As Expression) As Expression
            If left.Type.Equals(GetType(String)) AndAlso right.Type.equals(GetType(String)) Then
                Return GenerateStaticMethodCall("Concat", left, right)
            End If
            Return Expression.Add(left, right)
        End Function

        Function GenerateSubtract(ByVal left As Expression, ByVal right As expression) As Expression
            Return Expression.Subtract(Left, Right)
        End Function

        Function GenerateStringConcat(ByVal left As Expression, ByVal right As Expression) As Expression
            Return Expression.Call( _
                Nothing, _
                GetType(String).GetMethod("Concat", New Type() {GetType(Object), GetType(Object)}), _
                New Expression() {left, right})
        End Function

        Function GetStaticMethod(ByVal methodName As String, ByVal left As expression, ByVal right As expression) As MethodInfo
            Return left.Type.GetMethod(methodName, New Type() {left.Type, right.Type})
        End Function

        Function GenerateStaticMethodCall(ByVal methodName As String, ByVal left As Expression, ByVal right As Expression) As Expression
            Return Expression.Call(Nothing, GetStaticMethod(methodName, left, right), New Expression() {left, right})
        End Function

        Sub SetTextPos(ByVal pos As Integer)
            textPos = pos
            ch = If(textPos < textLen, text(textPos), ChrW(0))
        End Sub

        Sub NextChar()
            If textPos < textLen Then textPos += 1
            ch = If(textPos < textLen, text(textPos), ChrW(0))
        End Sub

        Sub NextToken()
            Do While Char.IsWhiteSpace(ch)
                NextChar()
            Loop

            Dim t As TokenId
            Dim tokenPos = textPos
            Select Case ch
                Case "!"c
                    NextChar()
                    If ch = "=" Then
                        NextChar()
                        t = TokenId.ExclamationEqual
                    Else
                        t = TokenId.Exclamation
                    End If
                Case "%"c
                    NextChar()
                    t = TokenId.Percent
                Case "&"c
                    NextChar()
                    If ch = "&" Then
                        NextChar()
                        t = TokenId.DoubleAmphersand
                    Else
                        t = TokenId.Amphersand
                    End If
                Case "("c
                    NextChar()
                    t = TokenId.OpenParen
                Case ")"c
                    NextChar()
                    t = TokenId.CloseParen
                Case "*"c
                    NextChar()
                    t = TokenId.Asterisk
                Case "+"c
                    NextChar()
                    t = TokenId.Plus
                Case ","c
                    NextChar()
                    t = TokenId.Comma
                Case "-"c
                    NextChar()
                    t = TokenId.Minus
                Case "."c
                    NextChar()
                    t = TokenId.Dot
                Case "/"c
                    NextChar()
                    t = TokenId.Slash
                Case ":"c
                    NextChar()
                    t = TokenId.Colon
                Case "<"c
                    NextChar()
                    If ch = "=" Then
                        NextChar()
                        t = TokenId.LessThanEqual
                    ElseIf ch = ">" Then
                        NextChar()
                        t = TokenId.LessGreater
                    Else
                        t = TokenId.LessThan
                    End If
                Case "="c
                    NextChar()
                    If ch = "=" Then
                        NextChar()
                        t = TokenId.DoubleEqual
                    Else
                        t = TokenId.Equal
                    End If
                Case ">"c
                    NextChar()
                    If ch = "=" Then
                        NextChar()
                        t = TokenId.GreaterThanEqual
                    Else
                        t = TokenId.GreaterThan
                    End If
                Case "?"c
                    NextChar()
                    t = TokenId.Question
                Case "["c
                    NextChar()
                    t = TokenId.OpenBracket
                Case "]"c
                    NextChar()
                    t = TokenId.CloseBracket
                Case "|"c
                    NextChar()
                    If ch = "|" Then
                        NextChar()
                        t = TokenId.DoubleBar
                    Else
                        t = TokenId.Bar
                    End If
                Case "'"c, """"c
                    Dim quote = ch
                    Do
                        NextChar()
                        Do While textPos < textLen AndAlso ch <> quote
                            NextChar()
                        Loop
                        If textPos = textLen Then Throw ParseError(textPos, Res.UnterminatedStringLiteral)
                        NextChar()
                    Loop While ch = quote
                    t = TokenId.StringLiteral
                Case Else
                    If Char.IsLetter(ch) OrElse ch = "@" OrElse ch = "_" Then
                        Do
                            NextChar()
                        Loop While Char.IsLetterOrDigit(ch) OrElse ch = "_"
                        t = TokenId.Identifier
                        Exit Select
                    End If

                    If Char.IsDigit(ch) Then
                        t = TokenId.IntegerLiteral
                        Do
                            NextChar()
                        Loop While Char.IsDigit(ch)
                        If ch = "." Then
                            t = TokenId.RealLiteral
                            NextChar()
                            ValidateDigit()
                            Do
                                NextChar()
                            Loop While Char.IsDigit(ch)
                        End If
                        If ch = "E" OrElse ch = "e" Then
                            t = TokenId.RealLiteral
                            NextChar()
                            If ch = "+" OrElse ch = "-" Then NextChar()
                            ValidateDigit()
                            Do
                                NextChar()
                            Loop While Char.IsDigit(ch)
                        End If
                        If ch = "F" Or ch = "f" Then NextChar()
                        Exit Select
                    End If
                    If textPos = textLen Then
                        t = TokenId.End
                        Exit Select
                    End If
                    Throw ParseError(textPos, Res.InvalidCharacter, ch)
            End Select
            tokenVal.id = t
            tokenVal.text = text.Substring(tokenPos, textPos - tokenPos)
            tokenVal.pos = tokenPos
        End Sub

        Function TokenIdentifierIs(ByVal id As String) As Boolean
            Return tokenVal.id = TokenId.Identifier AndAlso String.Equals(id, tokenVal.text, StringComparison.OrdinalIgnoreCase)
        End Function

        Function GetIdentifier() As String
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected)
            Dim id = tokenVal.text
            If id.Length > 1 AndAlso id(0) = "@" Then id = id.Substring(1)
            Return id
        End Function

        Sub ValidateDigit()
            If Not Char.IsDigit(ch) Then Throw ParseError(textPos, Res.DigitExpected)
        End Sub

        Sub ValidateToken(ByVal t As TokenId, ByVal errorMessage As String)
            If tokenVal.id <> t Then Throw ParseError(errorMessage)
        End Sub

        Sub ValidateToken(ByVal t As TokenId)
            If tokenVal.id <> t Then Throw ParseError(Res.SyntaxError)
        End Sub

        Overloads Function ParseError(ByVal format As String, ByVal ParamArray args As Object()) As Exception
            Return ParseError(tokenVal.pos, format, args)
        End Function

        Overloads Function ParseError(ByVal pos As Integer, ByVal format As String, ByVal ParamArray args As Object()) As Exception
            Return New ParseException(String.Format(System.Globalization.CultureInfo.CurrentCulture, format, args), pos)
        End Function

        Shared Function CreateKeywords() As Dictionary(Of String, Object)
            Dim d As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

            d.Add("true", trueLiteral)
            d.Add("false", falseLiteral)
            d.Add("null", nullLiteral)
            d.Add(keywordIt, keywordIt)
            d.Add(keywordIif, keywordIif)
            d.Add(keywordNew, keywordNew)

            For Each type As Type In predefinedTypes
                d.Add(type.Name, type)
            Next

            Return d
        End Function
    End Class

    Class Res
        Public Const DuplicateIdentifier As String = "The identifier '{0}' was defined more than once"
        Public Const ExpressionTypeMismatch As String = "Expression of type '{0}' expected"
        Public Const ExpressionExpected As String = "Expression expected"
        Public Const InvalidCharacterLiteral As String = "Character literal must contain exactly one character"
        Public Const InvalidIntegerLiteral As String = "Invalid integer literal '{0}'"
        Public Const InvalidRealLiteral As String = "Invalid real literal '{0}'"
        Public Const UnknownIdentifier As String = "Unknown identifier '{0}'"
        Public Const NoItInScope As String = "No 'it' is in scope"
        Public Const IifRequiresThreeArgs As String = "The 'iif' function requires three arguments"
        Public Const FirstExprMustBeBool As String = "The first expression must be of type 'Boolean'"
        Public Const BothTypesConvertToOther As String = "Both of the types '{0}' and '{1}' convert to the other"
        Public Const NeitherTypeConvertsToOther As String = "Neither of the types '{0}' and '{1}' converts to the other"
        Public Const MissingAsClause As String = "Expression is missing an 'as' clause"
        Public Const ArgsIncompatibleWithLambda As String = "Argument list incompatible with lambda expression"
        Public Const TypeHasNoNullableForm As String = "Type '{0}' has no nullable form"
        Public Const NoMatchingConstructor As String = "No matching constructor in type '{0}'"
        Public Const AmbiguousConstructorInvocation As String = "Ambiguous invocation of '{0}' constructor"
        Public Const CannotConvertValue As String = "A value of type '{0}' cannot be converted to type '{1}'"
        Public Const NoApplicableMethod As String = "No applicable method '{0}' exists in type '{1}'"
        Public Const MethodsAreInaccessible As String = "Methods on type '{0}' are not accessible"
        Public Const MethodIsVoid As String = "Method '{0}' in type '{1}' does not return a value"
        Public Const AmbiguousMethodInvocation As String = "Ambiguous invocation of method '{0}' in type '{1}'"
        Public Const UnknownPropertyOrField As String = "No property or field '{0}' exists in type '{1}'"
        Public Const NoApplicableAggregate As String = "No applicable aggregate method '{0}' exists"
        Public Const CannotIndexMultiDimArray As String = "Indexing of multi-dimensional arrays is not supported"
        Public Const InvalidIndex As String = "Array index must be an integer expression"
        Public Const NoApplicableIndexer As String = "No applicable indexer exists in type '{0}'"
        Public Const AmbiguousIndexerInvocation As String = "Ambiguous invocation of indexer in type '{0}'"
        Public Const IncompatibleOperand As String = "Operator '{0}' incompatible with operand type '{1}'"
        Public Const IncompatibleOperands As String = "Operator '{0}' incompatible with operand types '{1}' and '{2}'"
        Public Const UnterminatedStringLiteral As String = "Unterminated string literal"
        Public Const InvalidCharacter As String = "Syntax error '{0}'"
        Public Const DigitExpected As String = "Digit expected"
        Public Const SyntaxError As String = "Syntax error"
        Public Const TokenExpected As String = "{0} expected"
        Public Const ParseExceptionFormat As String = "{0} (at index {1})"
        Public Const ColonExpected As String = "':' expected"
        Public Const OpenParenExpected As String = "'(' expected"
        Public Const CloseParenOrOperatorExpected As String = "')' or operator expected"
        Public Const CloseParenOrCommaExpected As String = "')' or ',' expected"
        Public Const DotOrOpenParenExpected As String = "'.' or '(' expected"
        Public Const OpenBracketExpected As String = "'[' expected"
        Public Const CloseBracketOrCommaExpected As String = "']' or ',' expected"
        Public Const IdentifierExpected As String = "Identifier expected"
    End Class
End Namespace
