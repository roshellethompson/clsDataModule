Public Class clsTransaction
    Public Property TranID As Integer = 0
    Public Property tran_status As String = 0
    Public Property CreateDate As DateTime = Date.Now
    Public Property Amount As Double = 0.0
    Public Property TransactionTypeDescription As String
    Public Property TransactionTypeIcon As String
    Public Property TransactionTypeIcon2 As String
    Public Property tran_type As String = ""
    Public Property sec_tran_type As String = ""
    Public Property ActID As Integer = 0
    Public Property ActNbr As String = ""
    Public Property Act2Nbr As String = ""
    Public Property CustIsDest As Integer = 0
    Public Property BlockId As Integer = 0
    Public Property Description As String
    Public Property TransactionTypeDescription2 As String
    Public Property Fee As Decimal = 0
    Public Property Bal1 As Decimal = 0
    Public Property Bal2 As Decimal = 0
    Public Property CustomerID
    Public Property AmountReq As Decimal = 0
    Public Function Clone() As clsTransaction
        Return Me
    End Function
End Class
