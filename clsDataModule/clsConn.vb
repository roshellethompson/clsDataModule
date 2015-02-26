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

    Public Function Decrypt(ByVal strClear As String) As String
        Dim pwd As String
        pwd = TPSUtilities.AESEncryption.Decrypt(strClear, "tr@ns@ct", "123-98pnw-f9pcj9-qruk1-2uh0q34yh", "SHA1", 2, "16CHARSLONG12345", 256)
        Return pwd
    End Function

    Public Sub Close()
        sqlConn.Close()
        ifxConn.Close()
    End Sub

    Public Sub New()
        'Dim builderEZ As New System.Data.SqlClient.SqlConnectionStringBuilder
        'builderEZ("Data Source") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "DataSource", Nothing)
        'builderEZ("Initial Catalog") = "ezCash_Proto"
        'builderEZ("Persist Security Info") = True
        'builderEZ("Integrated Security") = "SSPI"
        'sqlConn = New SqlConnection(builderEZ.ConnectionString)
        'SQLConnStr = builderEZ.ConnectionString
        'Dim builderIFX As New IfxConnectionStringBuilder
        'builderIFX("Database") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\ODBC\ODBC.INI\switchware", "DATABASE", Nothing)
        'builderIFX("Host") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "InfxHost", Nothing)
        'builderIFX("Server") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\ODBC\ODBC.INI\switchware", "SERVER", Nothing)
        'builderIFX("Service") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "InfxService", Nothing)
        'builderIFX("Protocol") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "InfxProtocol", Nothing)
        'builderIFX("Password") = Decrypt(My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "InfxPwd", Nothing))
        'builderIFX("User ID") = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Transact\CardManagement\Settings", "InfxUser", Nothing)
        'ifxConn = New IfxConnection(builderIFX.ConnectionString)
        'IFXConnStr = builderIFX.ConnectionString
        sqlConn = New SqlConnection(ConfigurationManager.AppSettings("EzCash.ConnStr"))
        ifxConn = New IfxConnection(ConfigurationManager.AppSettings("ifxConn"))


    End Sub

    Private Shared Function GetConnectionStringByProvider( _
    ByVal providerName As String) As String

        'Return Nothing on failure. 
        Dim returnValue As String = Nothing

        ' Get the collection of connection strings. 
        Dim settings As ConnectionStringSettingsCollection = _
            ConfigurationManager.ConnectionStrings

        ' Walk through the collection and return the first  
        ' connection string matching the providerName. 
        If Not settings Is Nothing Then
            For Each cs As ConnectionStringSettings In settings
                If cs.ProviderName = providerName Then
                    returnValue = cs.ConnectionString
                    Exit For
                End If
            Next
        End If

        Return returnValue
    End Function

End Class
