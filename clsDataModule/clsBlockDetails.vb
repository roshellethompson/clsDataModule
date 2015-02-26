Public Class clsBlockDetails
    Public oTranRow As New List(Of clsTranRow)
    Public oFundAct As New List(Of clsTranRow)
    Public oHeaderRow As New List(Of clsTranRow)
    Public mBlockTotal As Decimal = 0
    Public mPaymentsTotal As Decimal = 0
    Public mFeeTotal As Decimal = 0
    Public dBlockDate As DateTime

    Public Sub New()

    End Sub
    Public Sub Init(ByVal oRow As clsTranRow, ByVal oFund As clsTranRow)
        oTranRow = oRow.Clone()
        oFund = oFund.Clone()

    End Sub
End Class
