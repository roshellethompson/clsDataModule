Imports System.Xml.Serialization
Imports System.Web.UI
<Serializable()>
Public Class clsTranRow

    Public Property oList As List(Of clsTranRow)
    Public Sub Add(ByVal o As System.Object)
        oList.Add(o)
    End Sub


    Public Property PlusLinkPostBack As String = ""
    Public Property BlockDetails As Object
    Public Property TranID As Integer = 0
    Public Property tran_status As String = 0
    Public Property cashBalance As Double = 0.0
    Public Property TransactionTypeDescription As String
    Public Property TransactionTypeIcon As String
    Public Property tran_type As String = ""

    Public Property ActID As Integer = 0
    Public Property short_desc As String = ""
    Public Property StatusIcon As String = ""
    Public Property ButtonText As String = ""
    Public Property PAN As String = ""
    Public Property CustIsDest As Boolean = 0

    Public Property ActTypeID As Integer = 0
    Public Property EntityID As Integer = 0
    Public Property AbleToDelete As Integer = 0
    Public Property ActNbr As String = ""
    Public Property date_time As DateTime = Date.Now
    Public Property BlockID As Integer = 0
    Public Property Description As String = ""
    Public Property Description2 As String = ""
    Public Property reg_e As String = ""
    Public Property RunBalance As Decimal
    Public Property CustID As Integer = 0
    Public Property Act2Number As String = ""
    Public Property BlockTotal As Decimal = 0
    Public Property Fee As Decimal = 0
    Public Property AmountReq As Decimal = 0
    Public Function Clone()
        Return Me
    End Function
    Public Sub New()

    End Sub


End Class
