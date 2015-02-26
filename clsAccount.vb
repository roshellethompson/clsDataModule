Public Class clsAccount
    Public Property ActID As Integer = 0
    Public Property short_desc As String = ""
    Public Property AcctIcon As String = ""

    Public Property ButtonText As String = ""
    Public Property PAN As String = ""

    Public Property ActTypeID As Integer = 0
    Public Property EntityID As Integer = 0
    Public Property AbleToDelete As Integer = 0
    Public Property ActNbr As String = ""
    Public Property cashBalance As Double
    Public Property CreateDate As DateTime = Date.Now
    Public Property IsEntity As Boolean = False
    Public Property CustomerID As Integer = 0
    Public Property DepositFlag As Integer = 0
    Public oTran As New clsTransaction()
    Public Function Clone(ByVal objTran As clsTransaction) As clsAccount
        oTran = objTran.Clone()
        Return Me
    End Function
End Class
