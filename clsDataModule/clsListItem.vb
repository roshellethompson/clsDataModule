Public Class clsListItem
    Private strText As String = ""
    Private strValue As String = ""
    Shared oItem As New clsListItem
    Public Property Text As String
        Get
            Return strText
        End Get
        Set(value As String)
            strText = value
        End Set
    End Property

    Public Property Value As String
        Get
            Return strValue
        End Get
        Set(value As String)
            strValue = value
        End Set
    End Property

    Public Shared ReadOnly Property GetTypeOf As System.Type
        Get
            Return oItem.GetType()
        End Get

    End Property

End Class
