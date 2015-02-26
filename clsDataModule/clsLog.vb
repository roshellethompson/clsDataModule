Imports System

Public Class clsLog
    Public fLog As System.IO.FileStream
    Public FileName As String

    ''' <summary>
    ''' Log activity to a text file
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <remarks></remarks>
    Public Sub LogMsg(ByVal strMsg As String)
        Try
            Close()
            fLog = New System.IO.FileStream(FileName, System.IO.FileMode.Append)

            strMsg = Date.Now.ToString + ": " + strMsg + vbCrLf
            Dim arrMsg(strMsg.Length) As Byte
            Dim i As Integer = 0
            For Each c In strMsg
                arrMsg(i) = AscW(c)
                i += 1
            Next
            fLog.Write(arrMsg, 0, arrMsg.Length)
            Close()
        Catch ex As Exception
            Close()
            LogMsg(strMsg)
        End Try

    End Sub

    Public Sub New(ByVal strFileName As String)
        FileName = strFileName
        If Not fLog Is Nothing Then
            fLog.Close()
        End If
        fLog = New System.IO.FileStream(FileName, System.IO.FileMode.Create)

    End Sub

    Public Sub Close()
        If Not fLog Is Nothing Then
            fLog.Close()
        End If
    End Sub

End Class
