Public Class clsEntityAccountType
    Public Property EntityID As Integer
    Public Property AccountTypeID As Integer
    Public Property RoutingNbr As String
    Public Property AccountNbr As String
    Public Property BankName As String
    Public Property BankAddressL1 As String
    Public Property BankAddressL2 As String
    Public Property BankCity As String
    Public Property BankState As String
    Public Property BankZip As String


    Public Sub Save()

        Dim oData As New clsData
        oData.SaveEntityActType(Me)
    End Sub
    Public Sub New()

    End Sub
    Public Sub New(ByVal iEntityID As Integer, ByVal iActTypeID As Integer)
        EntityID = iEntityID
        AccountTypeID = iActTypeID
    End Sub
End Class
