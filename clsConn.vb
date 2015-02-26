Imports System.Data.SqlClient
Imports IBM.Data.Informix
Imports System.Configuration
Public Class clsConn
    Public strConn As String = ""
    Public sqlConn As New SqlConnection(strConn)
    Public ifxConn As New IfxConnection
    Public Property SQLConnStr As String = ""
    Public Property IFXConnStr As String = ""

    Public Sub Connect(ByVal bIFX As Boolean)
        If bIFX Then
            If ifxConn.State = ConnectionState.Closed Then
                ifxConn.Open()
            End If
        End If
        If (Not bIFX) Then
            If sqlConn.State = ConnectionState.Closed Then
                sqlConn.Open()
            End If
        End If
    End Sub

    Public Sub Close()
        sqlConn.Close()
        ifxConn.Close()
    End Sub


    Public Sub New()
        sqlConn = New SqlConnection(ConfigurationManager.AppSettings("ConnStr"))
        Dim splitString As String = "transact"
        SQLConnStr = sqlConn.ConnectionString.Split(splitString)(0)
        ifxConn = New IfxConnection(ConfigurationManager.AppSettings("ifxConn"))
        IFXConnStr = ifxConn.ConnectionString.Split(splitString)(0)
    End Sub
End Class
