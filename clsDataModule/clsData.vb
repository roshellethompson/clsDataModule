Imports System.Data.SqlClient
Imports System.Xml
Imports IBM.Data.Informix
Imports System.Security.Cryptography
Imports System.ComponentModel
Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Configuration
Public Class clsData
    Shared strID As String = ""
    Shared strConn As String = ""
    Public Shared oItem As New clsListItem
    Public Property oConn As clsConn
    'Dim oJournal As New SaveJournal.Journal
    Dim oChkService As New CheckService.ICheckServiceservice



    Public Shared Property ConnectionString As String
        Get
            Return strConn
        End Get
        Set(ByVal value As String)
            strConn = value
        End Set
    End Property
    Public Shared Property ID As String
        Get
            Return strID
        End Get
        Set(ByVal value As String)
            strID = value
        End Set
    End Property

    Dim iScanID As Integer = 0
    ''' <summary>
    ''' The scan ID of the photo id image
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ScanID As Integer
        Get
            Return iScanID
        End Get
        Set(ByVal value As Integer)
            iScanID = value
        End Set
    End Property

    Public CaseNum As Integer = 0
    Public Function ValidateBin(ByVal iBin As Integer, ByVal iCustID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetTypeFromBin", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iBin", iBin)
        Dim parm2 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim litem As New clsListItem
        Dim strPan As String = ""
        Dim bValid As Boolean = False
        While dataReader.Read
            bValid = Trim(dataReader(0))
        End While
        dataReader.Close()
        oConn.Close()
        Return bValid
    End Function

    Public Function GetCurrentCaseNum() As Integer
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCurrentCaseNum", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim value As String = ""
        While dataReader.Read
            value = dataReader("CaseNum")
        End While
        dataReader.Close()
        oConn.Close()
        Return value
    End Function

    Public Function GetTestData(ByVal Key As String, ByVal CaseNum As Integer)


        'oJournal.Url = ConfigurationManager.AppSettings("SaveJournalURL")
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_QueryTestData", oConn.sqlConn)
        Dim parm1 As New SqlParameter("key", Key)
        Dim parm2 As New SqlParameter("caseNum", CaseNum)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim value As String = ""
        While dataReader.Read
            If IsDBNull(dataReader(1)) Then
                CaseNum = 0
            Else
                CaseNum = dataReader(1)
            End If


            If Not IsDBNull(dataReader(0)) And dataReader(0).ToString <> "" Then

                value = dataReader(0).ToString

            Else

                If IsNumeric(dataReader(0)) Then
                    value = 0
                Else
                    value = ""
                End If
                If Key.Contains("Date") Or Key.Contains("DOB") Then
                    value = Date.Now.ToString
                End If
            End If
        End While
        If Not dataReader.HasRows() Then
            value = ""
            If Key.Contains("Date") Or Key.Contains("DOB") Then
                value = Date.Now.ToString
            End If
        End If
        dataReader.Close()


        oConn.Close()
        UpdateRegTestData(Key)

        Return value
    End Function
    Public Function insChecks(ByVal iBlockID As Integer, ByVal iTransID As Integer, ByVal iCustomerID As Integer) As Boolean
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsChecks", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iBlockID", iBlockID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("iTransID", iTransID)
        sqlCmd.Parameters.Add(parm2)
        Dim parm3 As New SqlParameter("iCustomerID", iCustomerID)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True
    End Function
    Public Function GetAllEntities() As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select * from Entities where Active = 1", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetAllCompanies() As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCompanies", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetInactiveCompanies() As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetInactiveCompanies", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetInactiveCompaniesByEntityID(ByVal EntityID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetInactiveCompanies", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("EntityID", EntityID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetCompaniesByEntity(ByVal iEntityID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCompanies", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("EntityID", iEntityID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function GetCompaniesList() As List(Of clsCompany)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCompaniesList", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader = sqlCmd.ExecuteReader()
        Dim ret As New List(Of clsCompany)
        Dim oCompany As New clsCompany
        While dataReader.Read
            oCompany.CompanyName = dataReader("CompanyName")
            oCompany.CompanyNumber = dataReader("CompanyNumber")
            ret.Add(oCompany)
            oCompany = New clsCompany()
        End While
        Return ret
    End Function
    Public Function GetCompany(ByRef Company As clsCompany) As clsCompany
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCompanies", oConn.sqlConn)
        Dim parm1 As New SqlParameter("CompanyNumber", Company.CompanyNumber)
        sqlCmd.Parameters.Add(parm1)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            If Not IsDBNull(dataReader("CompanyName")) Then
                Company.CompanyName = dataReader("Companyname")
            End If
            If Not IsDBNull(dataReader("EntityID")) Then
                Company.EntityID = dataReader("EntityID")
            End If
            If Not IsDBNull(dataReader("EntityBK")) Then
                Company.EntityBK = dataReader("EntityBK")
            End If
            If Not IsDBNull(dataReader("EntityUS")) Then
                Company.EntityUS = dataReader("EntityUS")
            End If
            If Not IsDBNull(dataReader("EntityFN")) Then
                Company.EntityFN = dataReader("EntityFN")
            End If
            If Not IsDBNull(dataReader("EntityAT")) Then
                Company.EntityAT = dataReader("EntityAT")
            End If
            If Not IsDBNull(dataReader("DefaultCustGroupID")) Then
                Company.GroupID = dataReader("DefaultCustGroupID")
            End If
            If dataReader("Active") = 1 Then
                Company.Active = True
            Else
                Company.Active = False
            End If
        End While
        dataReader.Close()
        oConn.Close()
        Return Company
    End Function
    Public Function SaveCompany(ByVal oCompany As clsCompany) As Boolean
        oConn.Connect(False)

        Dim sqlCmd As New SqlCommand("sp_SaveCompany", oConn.sqlConn)
        Dim parm1 As New SqlParameter("EntityID", oCompany.EntityID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("CompName", oCompany.CompanyName)
        sqlCmd.Parameters.Add(parm2)
        Dim parm3 As New SqlParameter("HMActID", oCompany.HMActID)
        sqlCmd.Parameters.Add(parm3)
        Dim parm4 As New SqlParameter("Tier", oCompany.Tier)
        sqlCmd.Parameters.Add(parm4)
        Dim parm5 As New SqlParameter("EntityBK", oCompany.EntityBK)
        sqlCmd.Parameters.Add(parm5)
        Dim parm6 As New SqlParameter("EntityFN", oCompany.EntityFN)
        sqlCmd.Parameters.Add(parm6)
        Dim parm7 As New SqlParameter("EntityAT", oCompany.EntityAT)
        sqlCmd.Parameters.Add(parm7)
        Dim parm8 As New SqlParameter("EntityUS", oCompany.EntityUS)
        sqlCmd.Parameters.Add(parm8)
        Dim parm9 As New SqlParameter("CompNum", oCompany.CompanyNumber)
        sqlCmd.Parameters.Add(parm9)
        Dim parm10 As New SqlParameter("Active", oCompany.Active)
        sqlCmd.Parameters.Add(parm10)
        Dim parm11 As New SqlParameter("GroupID", oCompany.GroupID)
        sqlCmd.Parameters.Add(parm11)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True
    End Function
    Public Function SaveEntityActType(ByVal oEntActTYpe As clsEntityAccountType) As Boolean
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsEntityActType", oConn.sqlConn)
        Dim parm1 As New SqlParameter("EntityID", oEntActType.EntityID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("ActTypeID", oEntActTYpe.AccountTypeID)
        sqlCmd.Parameters.Add(parm2)
        Dim parm3 As New SqlParameter("RoutingNum", oEntActTYpe.RoutingNbr)
        sqlCmd.Parameters.Add(parm3)
        Dim parm4 As New SqlParameter("ActNum", oEntActTYpe.AccountNbr)
        sqlCmd.Parameters.Add(parm4)
        Dim parm5 As New SqlParameter("BankName", oEntActType.BankName)
        sqlCmd.Parameters.Add(parm5)
        Dim parm6 As New SqlParameter("Addr1", oEntActTYpe.BankAddressL1)
        sqlCmd.Parameters.Add(parm6)
        Dim parm7 As New SqlParameter("Addr2", oEntActTYpe.BankAddressL2)
        sqlCmd.Parameters.Add(parm7)
        Dim parm8 As New SqlParameter("City", oEntActTYpe.BankCity)
        sqlCmd.Parameters.Add(parm8)
        Dim parm9 As New SqlParameter("State", oEntActTYpe.BankState)
        sqlCmd.Parameters.Add(parm9)
        Dim parm10 As New SqlParameter("Zip", oEntActTYpe.BankZip)
        sqlCmd.Parameters.Add(parm10)

        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True
    End Function
    Public Function GetEntityActType(ByRef oEntActTYpe As clsEntityAccountType) As clsEntityAccountType
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetEntityActType", oConn.sqlConn)
        Dim parm1 As New SqlParameter("EntityID", oEntActTYpe.EntityID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("ActTypeID", oEntActTYpe.AccountTypeID)
        sqlCmd.Parameters.Add(parm2)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            If Not IsDBNull(dataReader("BankAddressL1")) Then
                oEntActTYpe.BankAddressL1 = dataReader("BankAddressL1")
            End If
            If Not IsDBNull(dataReader("BankAddressL2")) Then
                oEntActTYpe.BankAddressL2 = dataReader("BankAddressL2")
            End If
            If Not IsDBNull(dataReader("BankCity")) Then
                oEntActTYpe.BankCity = dataReader("BankCity")
            End If
            If Not IsDBNull(dataReader("BankName")) Then
                oEntActTYpe.BankName = dataReader("BankName")
            End If
            If Not IsDBNull(dataReader("BankState")) Then
                oEntActTYpe.BankState = dataReader("BankState")
            End If
            If Not IsDBNull(dataReader("BankZip")) Then
                oEntActTYpe.BankZip = dataReader("BankZip")
            End If
            If Not IsDBNull(dataReader("RoutingNbr")) Then
                oEntActTYpe.RoutingNbr = dataReader("RoutingNbr")
            End If
            If Not IsDBNull(dataReader("AccountNbr")) Then
                oEntActTYpe.AccountNbr = dataReader("AccountNbr")
            End If
        End While
        dataReader.Close()
        oConn.Close()
        Return oEntActTYpe
    End Function

    Public Function InsRegReview(ByVal Scan_ID As Integer, ByVal GroupID As Integer, ByVal CustomerID As Integer, ByVal First_name As String, _
ByVal Last_Name As String, ByVal DOB As DateTime, ByVal Middle_Name As String, ByVal State_Code As String, ByVal ID_Type As Integer, _
ByVal Issue_date As DateTime, ByVal Expiration_date As DateTime, ByVal PHOTO_ID As String, ByVal Address1 As String, ByVal Address2 As String, _
ByVal City As String, ByVal Zip As String, ByVal Height As String, ByVal Weight As String, ByVal Eye_Color As String, ByVal Hair_Color As String, _
ByVal User_Name As String, ByVal Status As String, ByVal CustomerImageVerify As Boolean, ByVal PhotoIDMatches As Boolean, ByVal Createdate As DateTime, _
ByVal SSN As String, ByVal Sex As String, ByVal TransactionType As String, ByVal BK_Flag As Integer, ByVal PAN As String) As Integer



        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsRegReview", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Scan_ID", Scan_ID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("GroupID", GroupID)
        sqlCmd.Parameters.Add(parm2)
        Dim parm3 As New SqlParameter("CustomerID", CustomerID)
        sqlCmd.Parameters.Add(parm3)
        Dim parm4 As New SqlParameter("First_name", First_name)
        sqlCmd.Parameters.Add(parm4)
        Dim parm5 As New SqlParameter("Last_Name", Last_Name)
        sqlCmd.Parameters.Add(parm5)
        Dim parm6 As New SqlParameter("DOB", DOB)
        sqlCmd.Parameters.Add(parm6)
        Dim parm7 As New SqlParameter("Middle_Name", Middle_Name)
        sqlCmd.Parameters.Add(parm7)
        Dim parm8 As New SqlParameter("State_Code", State_Code)
        sqlCmd.Parameters.Add(parm8)
        Dim parm9 As New SqlParameter("ID_Type", ID_Type)
        sqlCmd.Parameters.Add(parm9)
        Dim parm10 As New SqlParameter("Issue_date", Issue_date)
        sqlCmd.Parameters.Add(parm10)
        Dim parm11 As New SqlParameter("Expiration_date", Expiration_date)
        sqlCmd.Parameters.Add(parm11)
        Dim parm12 As New SqlParameter("Photo_ID", PHOTO_ID)
        sqlCmd.Parameters.Add(parm12)
        Dim parm13 As New SqlParameter("Address1", Address1)
        sqlCmd.Parameters.Add(parm13)
        Dim parm14 As New SqlParameter("Address2", Address2)
        sqlCmd.Parameters.Add(parm14)
        Dim parm15 As New SqlParameter("City", City)
        sqlCmd.Parameters.Add(parm15)
        Dim parm16 As New SqlParameter("Zip", Zip)
        sqlCmd.Parameters.Add(parm16)
        Dim parm17 As New SqlParameter("Height", Height)
        sqlCmd.Parameters.Add(parm17)
        Dim parm18 As New SqlParameter("Weight", Weight)
        sqlCmd.Parameters.Add(parm18)
        Dim parm19 As New SqlParameter("Eye_Color", Eye_Color)
        sqlCmd.Parameters.Add(parm19)
        Dim parm20 As New SqlParameter("Hair_Color", Hair_Color)
        sqlCmd.Parameters.Add(parm20)
        Dim parm21 As New SqlParameter("User_Name", Nothing)
        sqlCmd.Parameters.Add(parm21)
        Dim parm22 As New SqlParameter("Status", Status)
        sqlCmd.Parameters.Add(parm22)
        Dim parm23 As New SqlParameter("CustomerImageVerify", CustomerImageVerify)
        sqlCmd.Parameters.Add(parm23)
        Dim parm24 As New SqlParameter("PhotoIDMatches", PhotoIDMatches)
        sqlCmd.Parameters.Add(parm24)
        Dim parm25 As New SqlParameter("CreateDate", Createdate)
        sqlCmd.Parameters.Add(parm25)
        Dim parm26 As New SqlParameter("SSN", SSN)
        sqlCmd.Parameters.Add(parm26)
        Dim parm27 As New SqlParameter("Sex", Sex)
        sqlCmd.Parameters.Add(parm27)
        Dim parm28 As New SqlParameter("ReviewType", TransactionType)
        sqlCmd.Parameters.Add(parm28)
        Dim parm29 As New SqlParameter("Pan", PAN)
        sqlCmd.Parameters.Add(parm29)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim oID As Integer = 0
        While dataReader.Read
            oID = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()

        Return oID
    End Function
    Public Function FillLanguages() As ArrayList
        Dim arrLang As New ArrayList
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_FillLanguages", oConn.sqlConn)
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim litem As New clsListItem
        While dataReader.Read
            litem.Text = dataReader(0)
            litem.Value = dataReader(1)
            arrLang.Add(litem)
            litem = New clsListItem
        End While
        dataReader.Close()
        oConn.Close()
        Return arrLang
    End Function
    Public Sub UpdateRegTestData(ByVal Key)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateTestData", oConn.sqlConn)
        Dim parm1 As New SqlParameter("key", Key)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()

    End Sub
    Public Function GetTranTypes() As ArrayList
        oConn.Connect(False)
        Dim arrRet As New ArrayList
        Dim sqlCmd As New SqlCommand("sp_GetTranTypes", oConn.sqlConn)
        Dim oTran As New clsTransaction
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            oTran = New clsTransaction
            oTran.tran_type = dataReader(0)
            If IsDBNull(dataReader(1)) Then
                oTran.sec_tran_type = ""
            Else
                oTran.sec_tran_type = dataReader(1)
            End If
            arrRet.Add(oTran)
        End While
        dataReader.Close()
        oConn.Close()
        Return arrRet
    End Function
    Public Function GetWorkstationID(ByVal IP As String) As Integer
        oConn.Connect(False)
        Dim retVal As Integer = 0
        Dim sqlCmd As New SqlCommand("sp_GetWorkstationID", oConn.sqlConn)
        Dim parm1 As New SqlParameter("IP", IP)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function GetWorkstations() As ArrayList
        oConn.Connect(False)
        Dim arrRet As New ArrayList
        Dim sqlCmd As New SqlCommand("sp_GetWorkstations", oConn.sqlConn)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            arrRet.Add(dataReader(0))
        End While
        dataReader.Close()
        oConn.Close()
        Return arrRet
    End Function
    Public Function QuerycheckCodes(ByVal Key As String) As String


        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCheckCodeText", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Code", Key)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            retVal = dataReader("Description")

        End While
        dataReader.Close()
        oConn.Close()

        Return retVal
    End Function


    Public Function DropRegTestData() As Integer


        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_AlterRegTestData", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        oConn.Close()

        Return 0
    End Function
    Public Function UpdateRegReview(ByVal iScanID As Integer, ByRef strFirst As String, ByRef strlast As String, ByRef strDOB As String) As String


        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateRegReview", oConn.sqlConn)
        Dim parm1 As New SqlParameter("ScanID", iScanID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            strFirst = dataReader("First_Name")
            strlast = dataReader("Last_Name")
            strDOB = dataReader("DOB").ToString

        End While
        dataReader.Close()
        oConn.Close()

        Return retVal
    End Function
    Public Sub UpdateCheckReview(ByVal iBlockID As Integer, ByVal mAmt As Double, ByVal dCheckDate As Date)

        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateCheckReview", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iBlockID", iBlockID)
        Dim parm2 As New SqlParameter("mAmt", mAmt)
        Dim parm3 As New SqlParameter("dCheckDate", dCheckDate)

        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.CommandType = CommandType.StoredProcedure

        sqlCmd.ExecuteNonQuery()
        oConn.Close()

    End Sub
    Public Sub SaveImage(ByVal strCode As String, ByVal arrImg As Byte(), ByVal iCustID As Integer, ByVal iBlockID As Integer, _
                         ByVal iWksId As Integer, ByVal strPan As String, ByVal iTransID As Integer)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsertImage", oConn.sqlConn)
        Dim parm1 As New SqlParameter("arrImg", arrImg)
        Dim parm2 As New SqlParameter("iCustID", iCustID)
        Dim parm3 As New SqlParameter("iBlockID", iBlockID)
        Dim parm4 As New SqlParameter("iWksID", iWksId)
        Dim parm5 As New SqlParameter("vcCode", strCode)
        Dim parm6 As New SqlParameter("vcPan", strPan)
        Dim parm7 As New SqlParameter("iTransID", iTransID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.Parameters.Add(parm7)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
    End Sub
    Public Sub SaveCheckData(ByVal strCode As String, ByVal arrFSImg As Byte(), ByVal arrBSImg As Byte(), ByVal iCustID As Integer, ByVal iBlockID As Integer, _
                     ByVal iWksId As Integer, ByVal strRoute As String, ByVal strAcct As String, ByVal iCheckNum As Integer, ByVal mNetAmt As Decimal)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsCheckReviewPlusImage", oConn.sqlConn)
        Dim parm1 As New SqlParameter("arrFSImg", arrFSImg)
        Dim parm2 As New SqlParameter("iCustID", iCustID)
        Dim parm3 As New SqlParameter("iBlockID", iBlockID)
        Dim parm4 As New SqlParameter("iWksID", iWksId)
        Dim parm5 As New SqlParameter("vcCode", strCode)
        Dim parm6 As New SqlParameter("vcRoute", strRoute)
        Dim parm7 As New SqlParameter("vcAcct", strAcct)
        Dim parm8 As New SqlParameter("iCheckNum", iCheckNum)
        Dim parm9 As New SqlParameter("iCheckAmt", mNetAmt)
        Dim parm10 As New SqlParameter("arrBSImg", arrBSImg)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.Parameters.Add(parm7)
        sqlCmd.Parameters.Add(parm8)
        sqlCmd.Parameters.Add(parm9)
        sqlCmd.Parameters.Add(parm10)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
    End Sub

    Public Function SaveCustomerCard(ByVal iCustID As Integer, ByVal strPAN As String, ByVal strCDType As String, _
                              ByVal bLoadable As Boolean, ByVal ActNbr As String, ByVal CompanyID As Integer) As Integer
        Dim iActID As Integer = GetActID(ActNbr)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_InsertCustomerCard", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        Dim parm2 As New SqlParameter("strPan", strPAN)
        Dim parm3 As New SqlParameter("strCDType", strCDType)
        Dim parm5 As New SqlParameter("ActID", iActID)
        Dim parm6 As New SqlParameter("LoadableFlag", bLoadable)
        Dim parm7 As New SqlParameter("CompanyID", CompanyID)

        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.Parameters.Add(parm7)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As Integer = 0
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        oConn.Close()
        Return retVal
    End Function

    Public Function GetImage(ByVal iCustId As Integer, ByVal iBlockID As Integer, ByVal strCode As String, ByVal strPan As String) As Byte()
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetImage", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustId)
        Dim parm2 As New SqlParameter("iBlockID", iBlockID)
        Dim parm3 As New SqlParameter("vcCode", strCode)
        Dim parm4 As New SqlParameter("vcPan", strPan)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As Byte()
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function GetCheckCaseCode() As String

        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_QueryCheckCodes", oConn.sqlConn)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader = sqlCmd.ExecuteReader
        While dataReader.Read
            retVal = dataReader("Code")

        End While
        dataReader.Close()
        oConn.Close()

        Return retVal
    End Function

    Public Function UpdateCaseNum()


        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateCaseNum", oConn.sqlConn)
        Dim parm1 As New SqlParameter("caseNum", CaseNum)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        sqlCmd.ExecuteNonQuery()
        oConn.Close()

        Return retVal
    End Function
    Public Function GetCheckCodeText(ByVal strCode As String) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCheckCodeText", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Code", strCode)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function CreateTransactionNum(ByVal iCustId As Integer, ByVal tran_type As String, _
                                         ByVal sec_tran_type As String, ByVal iBlockID As Integer, _
                                         ByVal iActID As Integer, ByVal mAmt As Decimal) As XmlNode
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_CreateTransaction", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustId)
        Dim parm2 As New SqlParameter("tran_type", tran_type)
        Dim parm3 As New SqlParameter("sec_tran_type", sec_tran_type)
        Dim parm4 As New SqlParameter("iBlockID", iBlockID)
        Dim parm5 As New SqlParameter("iActID", iActID)
        Dim parm6 As New SqlParameter("dAmt", mAmt)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim ret1 As Integer = 0
        Dim ret2 As Integer = 0
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            ret1 = dataReader(0)
            ret2 = dataReader(1)
        End While
        dataReader.Close()
        oConn.Close()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Root", "")
        xmlDoc.AppendChild(root)
        Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "TransNum", "")
        xmlEle.InnerText = ret1.ToString
        Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Block_ID", "")
        xmlEle1.InnerText = ret2.ToString
        root.AppendChild(xmlEle)
        root.AppendChild(xmlEle1)
        Return xmlDoc.FirstChild
    End Function
    Public Function GetKioskSettings(ByVal strKey As String, ByVal iWksID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetKioskSettings", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Name", strKey)
        Dim parm2 As New SqlParameter("wksID", iWksID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function

    Public Function GetTranslation(ByVal strLabel As String, ByVal LangCode As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetTranslation", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Label", strLabel)
        Dim parm2 As New SqlParameter("LangCode", LangCode)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function GetSystemSettings(ByVal strKey As String) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetSystemSettings", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Setting", strKey)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function GetRegistrationDataFromPAN(ByVal strPan As String) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetRegistrationDataFromPAN", oConn.sqlConn)
        Dim parm1 As New SqlParameter("PAN", strPan)
        sqlCmd.Parameters.Add(parm1)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else : xmlEle.InnerText = ""
            End If

            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else : xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)
            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "SSN", "")
            If Not IsDBNull(dataReader("SSN")) Then
                xmlEle3.InnerText = dataReader("SSN")
            Else : xmlEle3.InnerText = ""
            End If

            root.AppendChild(xmlEle3)
            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PHOTO_ID", "")
            If Not IsDBNull(dataReader("Photo_ID")) Then
                xmlEle4.InnerText = dataReader("Photo_ID")
            Else
                xmlEle4.InnerText = ""
            End If
            root.AppendChild(xmlEle4)

            Dim xmlEle8 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Expiration_Date", "")
            If Not IsDBNull(dataReader("Expiration_Date")) Then

                xmlEle8.InnerText = dataReader("Expiration_Date")
            Else
                xmlEle8.InnerText = ""
            End If
            root.AppendChild(xmlEle8)
            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)

            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                xmlEle13.InnerText = dataReader("LangID")
            Else
                xmlEle13.InnerText = ""
            End If
            root.AppendChild(xmlEle13)

            Dim xmlEle15 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PAN", "")
            If Not IsDBNull(dataReader("PAN")) Then
                xmlEle15.InnerText = dataReader("PAN")
            Else
                xmlEle15.InnerText = ""
            End If
            root.AppendChild(xmlEle15)
            Dim xmlEle19 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle19.InnerText = dataReader("Zip")
            Else
                xmlEle19.InnerText = Nothing
            End If
            root.AppendChild(xmlEle19)
            Dim xmlEle20 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            If Not IsDBNull(dataReader("State_Code")) Then
                xmlEle20.InnerText = dataReader("State_Code")
            Else
                xmlEle20.InnerText = Nothing
            End If
            root.AppendChild(xmlEle20)
            Dim xmlEle21 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerID", "")
            If Not IsDBNull(dataReader("CustomerID")) Then
                xmlEle21.InnerText = dataReader("CustomerID")
            Else
                xmlEle21.InnerText = Nothing
            End If
            root.AppendChild(xmlEle21)
            Dim xmlEle22 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ScanID", "")
            If Not IsDBNull(dataReader("Scan_ID")) Then
                xmlEle22.InnerText = dataReader("Scan_ID")
            Else
                xmlEle22.InnerText = Nothing
            End If
            root.AppendChild(xmlEle22)
            Dim xmlEle23 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BGCheck", "")
            If Not IsDBNull(dataReader("BGCheck")) Then
                xmlEle23.InnerText = dataReader("BGCheck")
            Else
                xmlEle23.InnerText = Nothing
            End If
            root.AppendChild(xmlEle23)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild
    End Function
    Public Function GetCustomerDataFromLogin(ByVal strLogin As String) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromLogin", oConn.sqlConn)
        Dim parm1 As New SqlParameter("Login", strLogin)
        sqlCmd.Parameters.Add(parm1)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else : xmlEle.InnerText = ""
            End If

            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else
                xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)

            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)
            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                xmlEle13.InnerText = dataReader("LangID")
            Else
                xmlEle13.InnerText = ""
            End If
            root.AppendChild(xmlEle13)


            Dim xmlEle19 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle19.InnerText = dataReader("Zip")
            Else
                xmlEle19.InnerText = Nothing
            End If
            root.AppendChild(xmlEle19)
            Dim xmlEle20 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            If Not IsDBNull(dataReader("State_Code")) Then
                xmlEle20.InnerText = dataReader("State_Code")
            Else
                xmlEle20.InnerText = Nothing
            End If
            root.AppendChild(xmlEle20)
            Dim xmlEle21 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerID", "")
            If Not IsDBNull(dataReader("CustomerID")) Then
                xmlEle21.InnerText = dataReader("CustomerID")
            Else
                xmlEle21.InnerText = Nothing
            End If
            root.AppendChild(xmlEle21)
            Dim xmlEle22 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Registration_Source", "")
            If Not IsDBNull(dataReader("Registration_Source")) Then
                xmlEle22.InnerText = dataReader("Registration_Source")
            Else
                xmlEle22.InnerText = Nothing
            End If
            root.AppendChild(xmlEle22)
            Dim xmlEle23 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Registration_Flag", "")
            If Not IsDBNull(dataReader("Registration_Flag")) Then
                xmlEle23.InnerText = dataReader("Registration_Flag")
            Else
                xmlEle23.InnerText = Nothing
            End If
            root.AppendChild(xmlEle23)
            Dim xmlEle24 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BGCheck", "")
            If Not IsDBNull(dataReader("BGCheck")) Then
                xmlEle24.InnerText = dataReader("BGCheck")
            Else
                xmlEle24.InnerText = Nothing
            End If
            root.AppendChild(xmlEle24)
            Dim xmlEle25 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Customer_ACK", "")
            If Not IsDBNull(dataReader("Customer_ACK")) Then
                xmlEle25.InnerText = dataReader("Customer_ACK")
            Else
                xmlEle25.InnerText = Nothing
            End If
            root.AppendChild(xmlEle25)
            Dim xmlEle26 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LostCardCode", "")
            If Not IsDBNull(dataReader("LostCardCode")) Then
                xmlEle26.InnerText = dataReader("LostCardCode")
            Else
                xmlEle26.InnerText = Nothing
            End If
            root.AppendChild(xmlEle26)
            Dim Email As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Email", "")
            If Not IsDBNull(dataReader("Email")) Then
                Email.InnerText = dataReader("Email")
            Else
                Email.InnerText = Nothing
            End If
            root.AppendChild(Email)

            Dim strUserSalt As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "UserSalt", "")
            If Not IsDBNull(dataReader("user_salt")) Then
                strUserSalt.InnerText = dataReader("user_salt")
            Else
                strUserSalt.InnerText = Nothing
            End If
            root.AppendChild(strUserSalt)
            Dim strPassword As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Password", "")
            If Not IsDBNull(dataReader("Password")) Then
                strPassword.InnerText = dataReader("Password")
            Else
                strPassword.InnerText = Nothing
            End If
            root.AppendChild(strPassword)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild
    End Function
    Public Function GetCustomerDataFromPAN(ByVal strPan As String) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromPAN", oConn.sqlConn)
        Dim parm1 As New SqlParameter("PAN", strPan)
        sqlCmd.Parameters.Add(parm1)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else : xmlEle.InnerText = ""
            End If

            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else
                xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)

            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "SSN", "")
            xmlEle3.InnerText = dataReader("SSN")
            root.AppendChild(xmlEle3)

            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PHOTO_ID", "")
            If Not IsDBNull(dataReader("Photo_ID")) Then
                xmlEle4.InnerText = dataReader("Photo_ID")
            Else
                xmlEle4.InnerText = ""
            End If
            root.AppendChild(xmlEle4)

            Dim xmlEle8 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Expiration_Date", "")
            If Not IsDBNull(dataReader("Expiration_Date")) Then

                xmlEle8.InnerText = dataReader("Expiration_Date")
            Else
                xmlEle8.InnerText = ""
            End If
            root.AppendChild(xmlEle8)
            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)
            Dim xmlEle12 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ActID", "")
            If Not IsDBNull(dataReader("ActID")) Then
                xmlEle12.InnerText = dataReader("ActID")
            Else
                xmlEle12.InnerText = ""
            End If
            root.AppendChild(xmlEle12)
            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                xmlEle13.InnerText = dataReader("LangID")
            Else
                xmlEle13.InnerText = ""
            End If
            root.AppendChild(xmlEle13)

            Dim xmlEle15 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PAN", "")
            If Not IsDBNull(dataReader("PAN")) Then
                xmlEle15.InnerText = dataReader("PAN")
            Else
                xmlEle15.InnerText = ""
            End If
            root.AppendChild(xmlEle15)

            Dim xmlEle18 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CheckDate", "")
            If Not IsDBNull(dataReader("CheckDate")) Then
                xmlEle18.InnerText = dataReader("CheckDate")
            Else
                xmlEle18.InnerText = Nothing
            End If
            root.AppendChild(xmlEle18)
            Dim xmlEle19 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle19.InnerText = dataReader("Zip")
            Else
                xmlEle19.InnerText = Nothing
            End If
            root.AppendChild(xmlEle19)
            Dim xmlEle20 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            If Not IsDBNull(dataReader("State_Code")) Then
                xmlEle20.InnerText = dataReader("State_Code")
            Else
                xmlEle20.InnerText = Nothing
            End If
            root.AppendChild(xmlEle20)
            Dim xmlEle21 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerID", "")
            If Not IsDBNull(dataReader("CustomerID")) Then
                xmlEle21.InnerText = dataReader("CustomerID")
            Else
                xmlEle21.InnerText = Nothing
            End If
            root.AppendChild(xmlEle21)
            Dim xmlEle22 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Registration_Source", "")
            If Not IsDBNull(dataReader("Registration_Source")) Then
                xmlEle22.InnerText = dataReader("Registration_Source")
            Else
                xmlEle22.InnerText = Nothing
            End If
            root.AppendChild(xmlEle22)
            Dim xmlEle23 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Registration_Flag", "")
            If Not IsDBNull(dataReader("Registration_Flag")) Then
                xmlEle23.InnerText = dataReader("Registration_Flag")
            Else
                xmlEle23.InnerText = Nothing
            End If
            root.AppendChild(xmlEle23)
            Dim xmlEle24 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BGCheck", "")
            If Not IsDBNull(dataReader("BGCheck")) Then
                xmlEle24.InnerText = dataReader("BGCheck")
            Else
                xmlEle24.InnerText = Nothing
            End If
            root.AppendChild(xmlEle24)
            Dim xmlEle25 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Customer_ACK", "")
            If Not IsDBNull(dataReader("Customer_ACK")) Then
                xmlEle25.InnerText = dataReader("Customer_ACK")
            Else
                xmlEle25.InnerText = Nothing
            End If
            root.AppendChild(xmlEle25)
            Dim xmlEle26 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LostCardCode", "")
            If Not IsDBNull(dataReader("LostCardCode")) Then
                xmlEle26.InnerText = dataReader("LostCardCode")
            Else
                xmlEle26.InnerText = Nothing
            End If
            root.AppendChild(xmlEle26)
            Dim xmlEle27 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "cardID", "")
            If Not IsDBNull(dataReader("cardID")) Then
                xmlEle27.InnerText = dataReader("cardID")
            Else
                xmlEle27.InnerText = "0"
            End If
            root.AppendChild(xmlEle27)
            Dim Email As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Email", "")
            If Not IsDBNull(dataReader("Email")) Then
                Email.InnerText = dataReader("Email")
            Else
                Email.InnerText = Nothing
            End If
            root.AppendChild(Email)
            Dim CDType As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CDType", "")
            If Not IsDBNull(dataReader("CDType")) Then
                CDType.InnerText = dataReader("CDType")
            Else
                CDType.InnerText = "0"
            End If
            root.AppendChild(CDType)
            Dim ID_Code As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ID_Code", "")
            If Not IsDBNull(dataReader("ID_Code")) Then
                ID_Code.InnerText = dataReader("ID_Code")
            Else
                ID_Code.InnerText = ""
            End If
            root.AppendChild(ID_Code)
            Dim strUserSalt As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "UserSalt", "")
            If Not IsDBNull(dataReader("user_salt")) Then
                strUserSalt.InnerText = dataReader("user_salt")
            Else
                strUserSalt.InnerText = Nothing
            End If
            root.AppendChild(strUserSalt)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild
    End Function
    Public Function GetCustomerDataFromScan(ByVal iScanID As Integer) As XmlNode
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromScan", oConn.sqlConn)
        Dim parm1 As New SqlParameter("ScanID", iScanID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle12 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ScanID", "")
            xmlEle12.InnerText = iScanID
            root.AppendChild(xmlEle12)
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else
                xmlEle.InnerText = ""
            End If
            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else
                xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)
            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Address1", "")
            If Not IsDBNull(dataReader("Address1")) Then
                xmlEle3.InnerText = dataReader("Address1")
            Else
                xmlEle3.InnerText = ""
            End If
            root.AppendChild(xmlEle3)
            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PHOTO_ID", "")
            If Not IsDBNull(dataReader("PHOTO_ID")) Then
                xmlEle4.InnerText = dataReader("Photo_ID")
            Else
                xmlEle4.InnerText = ""
            End If
            root.AppendChild(xmlEle4)
            Dim xmlEle5 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "City", "")
            If Not IsDBNull(dataReader("City")) Then
                xmlEle5.InnerText = dataReader("City")
            Else
                xmlEle5.InnerText = ""
            End If
            root.AppendChild(xmlEle5)
            Dim xmlEle6 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle6.InnerText = dataReader("Zip")
            Else
                xmlEle6.InnerText = ""
            End If
            root.AppendChild(xmlEle6)
            Dim xmlEle7 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode", "")
            If Not IsDBNull(dataReader("ReturnCode")) Then
                xmlEle7.InnerText = dataReader("ReturnCode")
            Else
                xmlEle7.InnerText = ""
            End If
            root.AppendChild(xmlEle7)
            Dim xmlEle8 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Expiration_Date", "")
            If Not IsDBNull(dataReader("Expiration_Date")) Then
                xmlEle8.InnerText = dataReader("Expiration_Date")
            Else
                xmlEle8.InnerText = ""
            End If

            root.AppendChild(xmlEle8)
            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)
            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            xmlEle13.InnerText = dataReader("State_Code")
            root.AppendChild(xmlEle13)
            Dim xmlEle21 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BGCheck", "")
            If Not IsDBNull(dataReader("BGCheck")) Then
                xmlEle21.InnerText = dataReader("BGCheck")
            Else
                xmlEle21.InnerText = Nothing
            End If
            root.AppendChild(xmlEle21)
            Dim xmlEle25 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Customer_ACK", "")
            If Not IsDBNull(dataReader("Customer_ACK")) Then
                xmlEle25.InnerText = dataReader("Customer_ACK")
            Else
                xmlEle25.InnerText = Nothing
            End If
            root.AppendChild(xmlEle25)
            Dim xmlEle26 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LostCardCode", "")
            If Not IsDBNull(dataReader("LostCardCode")) Then
                xmlEle26.InnerText = dataReader("LostCardCode")
            Else
                xmlEle26.InnerText = Nothing
            End If
            root.AppendChild(xmlEle26)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild


    End Function
    Public Function GetCustomerDataFromPhotoID(ByVal strPhotoID As String, ByVal dDOB As DateTime, ByVal strState As String) As XmlNode
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromIDandDOB", oConn.sqlConn)
        Dim parm1 As New SqlParameter("PhotoID", strPhotoID)
        Dim parm2 As New SqlParameter("DOB", dDOB)
        Dim parm3 As New SqlParameter("State", strState)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else : xmlEle.InnerText = ""
            End If

            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else : xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)
            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "SSN", "")
            If Not IsDBNull(dataReader("SSN")) Then
                xmlEle3.InnerText = dataReader("SSN")
            End If
            root.AppendChild(xmlEle3)
            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PHOTO_ID", "")
            If Not IsDBNull(dataReader("Photo_ID")) Then
                xmlEle4.InnerText = dataReader("Photo_ID")
            Else
                xmlEle4.InnerText = ""
            End If
            root.AppendChild(xmlEle4)

            Dim xmlEle8 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Expiration_Date", "")
            If Not IsDBNull(dataReader("Expiration_Date")) Then

                xmlEle8.InnerText = dataReader("Expiration_Date")
            Else
                xmlEle8.InnerText = ""
            End If
            root.AppendChild(xmlEle8)
            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)
            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                xmlEle13.InnerText = dataReader("LangID")
            Else
                xmlEle13.InnerText = ""
            End If
            root.AppendChild(xmlEle13)

            Dim xmlEle15 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PAN", "")
            If Not IsDBNull(dataReader("PAN")) Then
                xmlEle15.InnerText = dataReader("PAN")
            Else
                xmlEle15.InnerText = ""
            End If
            root.AppendChild(xmlEle15)

            Dim xmlEle19 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle19.InnerText = dataReader("Zip")
            Else
                xmlEle19.InnerText = Nothing
            End If
            root.AppendChild(xmlEle19)
            Dim xmlEle20 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            If Not IsDBNull(dataReader("State_Code")) Then
                xmlEle20.InnerText = dataReader("State_Code")
            Else
                xmlEle20.InnerText = Nothing
            End If
            root.AppendChild(xmlEle20)
            Dim xmlEle21 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerID", "")
            If Not IsDBNull(dataReader("CustomerID")) Then
                xmlEle21.InnerText = dataReader("CustomerID")
            Else
                xmlEle21.InnerText = Nothing
            End If
            root.AppendChild(xmlEle21)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild


    End Function
    Public Function HasCheckBeenReviewed(ByVal strAcct As String, ByVal strRoute As String, ByVal strCheckNum As String, ByVal iCustID As Integer, ByVal iWksID As Integer) As XmlNode
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_HasCheckBeenReviewed", oConn.sqlConn)
        Dim parm1 As New SqlParameter("vcAcct", strAcct)
        Dim parm2 As New SqlParameter("vcRoute", strRoute)
        Dim parm3 As New SqlParameter("vcCheckNum", strCheckNum)
        Dim parm4 As New SqlParameter("iCustID", iCustID)
        Dim parm5 As New SqlParameter("iWksID", iWksID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read

            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BlockID", "")
            xmlEle.InnerText = dataReader(0)
            root.AppendChild(xmlEle)



        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild


    End Function
    Public Function GetErrorData(ByVal iErrCode As Integer) As XmlElement

        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetErrData", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iErrCode", iErrCode)
        sqlCmd.Parameters.Add(parm1)

        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read

            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrCode", "")
            xmlEle.InnerText = dataReader("ErrCode")
            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrText", "")
            xmlEle1.InnerText = dataReader("ErrText")
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Component", "")
            xmlEle2.InnerText = dataReader("Component")
            root.AppendChild(xmlEle2)
            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Action", "")
            xmlEle3.InnerText = dataReader("Action")
            root.AppendChild(xmlEle3)

        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild


    End Function

    Public Function leftPad(ByVal s As String, ByVal len As Integer, ByVal PadChar As String) As String
        Dim result As String = s
        While result.Length < len
            result = PadChar + result
        End While
        Return result
    End Function

    Public Function GetUserSalt(ByVal CustomerID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select user_salt from Customer where CustomerID = " + CustomerID.ToString, oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader = sqlCmd.ExecuteReader
        Dim oRet As String = ""
        'updated by Roshelle 10/16 - null error handling
        While dataReader.Read
            If Not IsDBNull(dataReader(0)) Then
                oRet = dataReader(0)
            Else
                oRet = ""
            End If
        End While
        dataReader.Close()
        oConn.Close()
        Return oRet
    End Function
    Public Function GetCheckData(ByVal iCustID As Integer, ByVal iBlockID As Integer) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCheckData", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        Dim parm2 As New SqlParameter("iBlockID", iBlockID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read

            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Amount", "")
            If Not IsDBNull(dataReader("Amount")) Then
                xmlEle.InnerText = dataReader("Amount")
            Else
                xmlEle.InnerText = "0"
            End If
            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ActNbr", "")
            If Not IsDBNull(dataReader("ActNbr")) Then
                xmlEle1.InnerText = dataReader("ActNbr")
            Else
                xmlEle1.InnerText = "0"
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CheckNbr", "")
            If Not IsDBNull(dataReader("CheckNbr")) Then
                xmlEle2.InnerText = dataReader("CheckNbr")
            Else
                xmlEle2.InnerText = "0"
            End If
            root.AppendChild(xmlEle2)
            Dim xmlele3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Routing_Number", "")
            If Not IsDBNull(dataReader("Routing_Number")) Then
                xmlele3.InnerText = dataReader("Routing_Number")
            Else
                xmlele3.InnerText = "0"
            End If
            root.AppendChild(xmlele3)
            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Date", "")
            If Not IsDBNull(dataReader("Date")) Then
                xmlEle4.InnerText = dataReader("Date")
            Else
                xmlEle4.InnerText = "0"
            End If
            root.AppendChild(xmlEle4)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild


    End Function
    Public Function GetCustomerDataFromIDForCOMMS(ByVal strPhotoID As String, ByVal dDOB As DateTime, ByVal strState As String,
                                         ByVal iCustomerID As Integer, ByVal strCountry As String, ByVal iWksID As Integer) As XmlNode

        oConn.Connect(False)
        'Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromID", oConn.sqlConn)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataForCOMMS", oConn.sqlConn)

        oChkService.Url = ConfigurationManager.AppSettings("CheckServiceURL")
        oChkService.Trace("sql conn string:" + oConn.SQLConnStr.ToString())


        Dim parm1 As New SqlParameter("iCustID", iCustomerID)
        Dim parm2 As New SqlParameter("iWksID", iWksID)
        Dim parm3 As New SqlParameter("PhotoID", strPhotoID)
        Dim parm4 As New SqlParameter("DOB", dDOB)
        Dim parm5 As New SqlParameter("State", strState)
        Dim parm6 As New SqlParameter("Country", strCountry)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read

            Dim iLangID As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "iLangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                iLangID.InnerText = dataReader("LangID")
            Else
                iLangID.InnerText = "1"
            End If
            root.AppendChild(iLangID)
            Dim MessageSMS As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageSMS", "")
            If Not IsDBNull(dataReader("MessageSMS")) Then
                MessageSMS.InnerText = dataReader("MessageSMS")
            Else
                MessageSMS.InnerText = Nothing
            End If
            root.AppendChild(MessageSMS)
            Dim MessageEmail As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageEmail", "")
            If Not IsDBNull(dataReader("MessageEmail")) Then
                MessageEmail.InnerText = dataReader("MessageEmail")
            Else
                MessageEmail.InnerText = Nothing
            End If
            root.AppendChild(MessageEmail)
            Dim MessageLetter As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageLetter", "")
            If Not IsDBNull(dataReader("MessageLetter")) Then
                MessageLetter.InnerText = dataReader("MessageLetter")
            Else
                MessageLetter.InnerText = Nothing
            End If
            root.AppendChild(MessageLetter)
            Dim PhoneMobile As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhoneMobile", "")
            If Not IsDBNull(dataReader("PhoneMobile")) Then
                PhoneMobile.InnerText = dataReader("PhoneMobile")
                oChkService.Trace("Customer Phone: " + PhoneMobile.InnerText)
            Else
                PhoneMobile.InnerText = Nothing
            End If
            root.AppendChild(PhoneMobile)
            Dim Email As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Email", "")
            If Not IsDBNull(dataReader("Email")) Then
                Email.InnerText = dataReader("Email")
            Else
                Email.InnerText = Nothing
            End If
            root.AppendChild(Email)



        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild

    End Function

    Public Function GetCustomerDataFromID(ByVal strPhotoID As String, ByVal dDOB As DateTime, ByVal strState As String,
                                          ByVal iCustomerID As Integer, ByVal strRouteNumber As String, _
                                          ByVal strAccountNumber As String, ByVal iCheckNumber As Integer, _
                                          ByVal mNetAmt As Decimal, ByVal iWksID As Integer) As XmlNode

        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustomerDataFromID", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustomerID)
        Dim parm2 As New SqlParameter("vcRoute", strRouteNumber)
        Dim parm3 As New SqlParameter("vcAcct", strAccountNumber)
        Dim parm4 As New SqlParameter("iWksID", iWksID)
        Dim parm5 As New SqlParameter("iCheckNum", iCheckNumber)
        Dim parm6 As New SqlParameter("iCheckAmt", mNetAmt)
        Dim parm7 As New SqlParameter("PhotoID", strPhotoID)
        Dim parm8 As New SqlParameter("DOB", dDOB)
        Dim parm9 As New SqlParameter("State", strState)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.Parameters.Add(parm6)
        sqlCmd.Parameters.Add(parm7)
        sqlCmd.Parameters.Add(parm8)
        sqlCmd.Parameters.Add(parm9)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DOB", "")
            If Not IsDBNull(dataReader("DOB")) Then
                xmlEle.InnerText = dataReader("DOB")
            Else : xmlEle.InnerText = ""
            End If

            root.AppendChild(xmlEle)
            Dim xmlEle1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "First", "")
            If Not IsDBNull(dataReader("First_name")) Then
                xmlEle1.InnerText = dataReader("First_name")
            Else
                xmlEle1.InnerText = ""
            End If
            root.AppendChild(xmlEle1)
            Dim xmlEle2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Last", "")
            If Not IsDBNull(dataReader("Last_name")) Then
                xmlEle2.InnerText = dataReader("Last_name")
            Else : xmlEle2.InnerText = ""
            End If
            root.AppendChild(xmlEle2)
            Dim xmlEle3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "SSN", "")
            If Not IsDBNull(dataReader("SSN")) Then
                xmlEle3.InnerText = dataReader("SSN")
            Else
                xmlEle3.InnerText = ""
            End If
            root.AppendChild(xmlEle3)
            Dim xmlEle4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PHOTO_ID", "")
            If Not IsDBNull(dataReader("Photo_ID")) Then
                xmlEle4.InnerText = dataReader("Photo_ID")
            Else
                xmlEle4.InnerText = ""
            End If
            root.AppendChild(xmlEle4)
            Dim xmlEle7 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode2", "")
            If Not IsDBNull(dataReader("ReturnCode2")) Then
                xmlEle7.InnerText = dataReader("ReturnCode2")
            Else
                xmlEle7.InnerText = ""
            End If
            root.AppendChild(xmlEle7)
            Dim xmlEle8 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Expiration_Date", "")
            If Not IsDBNull(dataReader("Expiration_Date")) Then

                xmlEle8.InnerText = dataReader("Expiration_Date")
            Else
                xmlEle8.InnerText = ""
            End If
            root.AppendChild(xmlEle8)
            Dim xmlEle9 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Status", "")
            If Not IsDBNull(dataReader("Status")) Then
                xmlEle9.InnerText = dataReader("Status")
            Else
                xmlEle9.InnerText = ""
            End If
            root.AppendChild(xmlEle9)
            Dim xmlEle10 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhotoIDMatches", "")
            If Not IsDBNull(dataReader("PhotoIDMatches")) Then
                xmlEle10.InnerText = dataReader("PhotoIDMatches")
            Else
                xmlEle10.InnerText = ""
            End If
            root.AppendChild(xmlEle10)
            Dim xmlEle11 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerImageVerify", "")
            If Not IsDBNull(dataReader("CustomerImageVerify")) Then
                xmlEle11.InnerText = dataReader("CustomerImageVerify")
            Else
                xmlEle11.InnerText = ""
            End If
            root.AppendChild(xmlEle11)
            Dim xmlEle12 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "DefaultAccount", "")
            If Not IsDBNull(dataReader("DefaultAccount")) Then
                xmlEle12.InnerText = dataReader("DefaultAccount")
            Else
                xmlEle12.InnerText = ""
            End If
            root.AppendChild(xmlEle12)
            Dim xmlEle13 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                xmlEle13.InnerText = dataReader("LangID")
            Else
                xmlEle13.InnerText = ""
            End If
            root.AppendChild(xmlEle13)

            Dim xmlEle15 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PAN", "")
            If Not IsDBNull(dataReader("PAN")) Then
                xmlEle15.InnerText = dataReader("PAN")
            Else
                xmlEle15.InnerText = ""
            End If
            root.AppendChild(xmlEle15)

            Dim xmlEle18 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CheckDate", "")
            If Not IsDBNull(dataReader("CheckDate")) Then
                xmlEle18.InnerText = dataReader("CheckDate")
            Else
                xmlEle18.InnerText = Nothing
            End If
            root.AppendChild(xmlEle18)
            Dim xmlEle19 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Zip", "")
            If Not IsDBNull(dataReader("Zip")) Then
                xmlEle19.InnerText = dataReader("Zip")
            Else
                xmlEle19.InnerText = Nothing
            End If
            root.AppendChild(xmlEle19)
            Dim xmlEle20 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "State", "")
            If Not IsDBNull(dataReader("State_Code")) Then
                xmlEle20.InnerText = dataReader("State_Code")
            Else
                xmlEle20.InnerText = Nothing
            End If
            root.AppendChild(xmlEle20)
            Dim MessageSMS As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageSMS", "")
            If Not IsDBNull(dataReader("MessageSMS")) Then
                MessageSMS.InnerText = dataReader("MessageSMS")
            Else
                MessageSMS.InnerText = Nothing
            End If
            root.AppendChild(MessageSMS)
            Dim MessageEmail As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageEmail", "")
            If Not IsDBNull(dataReader("MessageEmail")) Then
                MessageEmail.InnerText = dataReader("MessageEmail")
            Else
                MessageEmail.InnerText = Nothing
            End If
            root.AppendChild(MessageEmail)
            Dim MessageLetter As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MessageLetter", "")
            If Not IsDBNull(dataReader("MessageLetter")) Then
                MessageLetter.InnerText = dataReader("MessageLetter")
            Else
                MessageLetter.InnerText = Nothing
            End If
            root.AppendChild(MessageLetter)
            Dim PhoneMobile As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "PhoneMobile", "")
            If Not IsDBNull(dataReader("PhoneMobile")) Then
                PhoneMobile.InnerText = dataReader("PhoneMobile")
            Else
                PhoneMobile.InnerText = Nothing
            End If
            root.AppendChild(PhoneMobile)
            Dim Email As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Email", "")
            If Not IsDBNull(dataReader("Email")) Then
                Email.InnerText = dataReader("Email")
            Else
                Email.InnerText = Nothing
            End If
            root.AppendChild(Email)
            Dim LostCardCode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "LostCardCode", "")
            If Not IsDBNull(dataReader("LostCardCode")) Then
                LostCardCode.InnerText = dataReader("LostCardCode")
            Else
                LostCardCode.InnerText = Nothing
            End If
            root.AppendChild(LostCardCode)
            Dim iLangID As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "iLangID", "")
            If Not IsDBNull(dataReader("LangID")) Then
                iLangID.InnerText = dataReader("LangID")
            Else
                iLangID.InnerText = Nothing
            End If
            root.AppendChild(iLangID)
            Dim strUserSalt As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "UserSalt", "")
            If Not IsDBNull(dataReader("user_salt")) Then
                strUserSalt.InnerText = dataReader("user_salt")
            Else
                strUserSalt.InnerText = Nothing
            End If
            root.AppendChild(strUserSalt)
            Dim CardID As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CardID", "")
            If Not IsDBNull(dataReader("CardID")) Then
                CardID.InnerText = dataReader("CardID")
            Else
                CardID.InnerText = Nothing
            End If
            root.AppendChild(CardID)
        End While
        dataReader.Close()
        oConn.Close()
        Return xmlDoc.FirstChild

    End Function

    Public Function GetActID(ByVal ActNbr As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select ActID from Accounts where ActNbr = " + ActNbr.ToString(), oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim retInt As String = ""
        While dataReader.Read
            retInt = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retInt
    End Function
    Public Function GetCustomerPhotoID(ByVal Barcode As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select * from CustomerPhotoIDs where Barcode = " + Barcode.ToString(), oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim dt As New DataTable
        dt.Columns.Add("PhotoID")
        dt.Columns.Add("DOB")
        dt.Columns.Add("State")
        dt.Columns.Add("ID")
        Dim photoID As String = ""
        Dim DOB As String = ""
        Dim State As String = ""
        Dim ID As Integer = 0
        While (dataReader.Read)
            photoID = dataReader("DLNbr")
            DOB = dataReader("DOB")
            State = dataReader("State")
            ID = dataReader("ID")
            If (photoID <> "" And DOB <> "" And State <> "") Then
                dt.Rows.Add(New Object() {ID, photoID, DOB, State})
                Exit While
            End If
        End While
        dataReader.Close()
        oConn.Close()
        Dim ds As New DataSet
        ds.Tables.Add(dt)
        Return ds

    End Function

    Public Function SetCustomerPhotoID(ByVal PhotoIDsID As Integer, ByVal CustomerID As Integer)
        oConn.Connect(False)
        Dim strZero As String = "0"
        Dim sqlCmd As New SqlCommand("Update CustomerPhotoIDs set CustomerID = '" + strZero + "' where CustomerID = '" + CustomerID.ToString + "'", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        sqlCmd.ExecuteNonQuery()
        sqlCmd = New SqlCommand("Update CustomerPhotoIDs set CustomerID = '" + CustomerID.ToString() + "' where ID = " + PhotoIDsID.ToString() + "", oConn.sqlConn)
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
    End Function
    Public Sub UpdateCustomer(ByVal Password As String, ByVal iCustID As Integer)
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateCustomer", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("pwd", Password)
        Dim parm2 As New SqlParameter("custID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
    End Sub
    Public Function GetCard(ByVal ActID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select PAN from CustomerCards where ActID = " + ActID.ToString(), oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim retInt As String = ""
        While dataReader.Read
            retInt = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retInt
    End Function
    Public Function GetCustFromAccount(ByVal ActID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select CustomerID from Accounts where ActID = " + ActID.ToString(), oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim retInt As String = ""
        While dataReader.Read
            retInt = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retInt
    End Function
    Public Function GetActNbr(ByVal ActID As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("Select ActNbr from Accounts where ActID = " + ActID.ToString(), oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader
        Dim retInt As String = ""
        While dataReader.Read
            retInt = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retInt
    End Function
    Public Function SelectPendingReviews() As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetPendingReviews", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function GetAllTransactionsbyBlockAndCust(ByVal iBlockID As Integer, ByVal iCustID As Integer, ByVal iTranID As Integer, ByVal acctDs As DataSet, ByVal feeCompDS As DataSet) As clsBlockDetails
        '   Dim acctDS As DataSet = GetEZCashAccounts(iCustID)
        '   Dim feeCompDS As DataSet = IFX_GetFeeWebComp()
        Dim oBlockDet As New clsBlockDetails
        Dim dt As New DataTable("AllTransactions")
        Dim arrRes As New ArrayList
        Dim ds As New DataSet
        Dim arrDep As New ArrayList
        Dim balance1 As String
        Dim balance2 As String

        'Gets Accounts where DepositFlag=1
        Dim dsDep As DataSet = GetEZCashDepositAccounts(iCustID)
        For Each objRow In dsDep.Tables(0).Rows
            Dim aAcct As New clsAccount
            For Each objCol In dsDep.Tables(0).Columns
                If objCol.Caption = "ActNbr" Then
                    aAcct.ActNbr = objRow(objCol)
                ElseIf objCol.Caption = "DepositFlag" Then
                    aAcct.DepositFlag = objRow(objCol)
                ElseIf objCol.Caption = "ButtonText" Then
                    aAcct.ButtonText = objRow(objCol)
                ElseIf objCol.Caption = "ActID" Then
                    aAcct.ActID = objRow(objCol)
                End If
            Next
           
            arrDep.Add(aAcct)
        Next

        'All accounts for customer
        
        Dim arrAcct As New ArrayList
        For Each objRow In acctDs.Tables(0).Rows
            Dim aAcct As New clsAccount
            For Each objCol In acctDs.Tables(0).Columns
                If objCol.Caption = "ActNbr" Then
                    aAcct.ActNbr = objRow(objCol)
                ElseIf objCol.Caption = "DepositFlag" Then
                    aAcct.DepositFlag = objRow(objCol)
                ElseIf objCol.Caption = "ButtonText" Then
                    aAcct.ButtonText = objRow(objCol)
                ElseIf objCol.Caption = "ActID" Then
                    aAcct.ActID = objRow(objCol)
                End If
            Next
            arrAcct.Add(aAcct)
           
        Next


        Dim arrTrans As New ArrayList
        Dim arrIFXAcct As New ArrayList()
        Dim arrHeader As New ArrayList
        Dim ifxCol As New DataColumn

        'Gets all transactions in block from Informex
        Dim dsIFX As DataSet = IFX_GetTransactionsByBlock(iBlockID, iCustID)
        
        Dim bMatch As Boolean = False
        For Each oIFXRow In dsIFX.Tables(0).Rows
            Dim oAcct As New clsAccount()
            Dim l As Integer = 0
            Dim bAcctmatch As Boolean = False
            Dim bAcct1Dep As Boolean = False
            Dim bAcct2match As Boolean = False
            Dim bAcct2Dep As Boolean = False
            Dim oAct1Acct As New clsAccount
            Dim oAct2Acct As New clsAccount

            'oAcct - account object for each transaction in block
            oAcct.CustomerID = iCustID
            Dim bCustmatch As Boolean = False
            For Each oIFXCol In dsIFX.Tables(0).Columns
                If oIFXCol.Caption = "amount_auth" Then
                    oAcct.oTran.Amount = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "amount_req" Then
                    oAcct.oTran.AmountReq = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "tp_datetime" Then
                    oAcct.oTran.CreateDate = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pri_tran_code" Then
                    oAcct.oTran.tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "sec_tran_code" Then
                    oAcct.oTran.sec_tran_type = oIFXRow(oIFXCol)
                    'acct_1_num is the 'From' acct
                ElseIf oIFXCol.Caption = "acct_1_nbr" Then
                    oAcct.oTran.ActNbr = oIFXRow(oIFXCol)
                    'acct_2_num is the 'To' account
                ElseIf oIFXCol.Caption = "acct_2_nbr" Then
                    oAcct.oTran.Act2Nbr = oIFXRow(oIFXCol)
                    l = 0
                    'compare every account the customer has to the accounts used in the transaction row
                    While l < arrAcct.Count
                        Dim oAccount = New clsAccount
                        oAccount = arrAcct(l).Clone(arrAcct(l).oTran)
                        ' Compare the 'From' account number to the acct number (which is ecrypted in EZCash's Accounts DB)
                        ' Account num is EZCash is 9, but is 18 in SWX, so pad EZCash acct
                        If oAcct.oTran.ActNbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then

                            bAcctmatch = True
                            'oAct1Acct is a clone of the Account row in EZCash.Accounts table
                            oAct1Acct = oAccount.Clone(oAccount.oTran)
                            If oAct1Acct.DepositFlag Then
                                ' baAcct1Dep = The Account is the 'from' and the depositflag is true
                                bAcct1Dep = True
                            End If
                        End If
                        '   if the 'To' Account number is the account ID of the row in Accounts table
                        If Int32.Parse(oAcct.oTran.Act2Nbr) = oAccount.ActID Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                            ' if the 'To' Account number encrypted matches the Acct number from Accounts table 
                        ElseIf oAcct.oTran.Act2Nbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                        End If
                        l += 1
                    End While
                    ' match on the From account
                    If bAcctmatch And Not bAcct2match Then
                        oAcct.ButtonText = oAct1Acct.ButtonText
                        oAcct.ActID = oAct1Acct.ActID
                        oAcct.DepositFlag = oAct1Acct.DepositFlag
                        If oAcct.CustomerID <> oAct1Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct1Acct.ActNbr
                        'money is being moved from so it is neg
                        oAcct.oTran.Amount = -oAcct.oTran.Amount
                        ' match on the To account
                    ElseIf bAcct2match Then
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAct2Acct.DepositFlag Then
                            oAcct.ActID = 0
                        End If
                    Else
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAcct.CustomerID <> oAct2Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        bAcct2Dep = True
                    End If

                ElseIf oIFXCol.Caption = "tranid" Then
                    oAcct.oTran.TranID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "type_code" Then
                    oAcct.oTran.tran_status = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pos_merch_nbr" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.BlockId = 0
                    Else
                        oAcct.oTran.BlockId = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "tp_reg_e" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.reg_e = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description2" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description2 = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription2" Then
                    If bAcct2match Then

                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct1icon" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct2icon" Then
                    If bAcct2match Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If
                    End If
                ElseIf oIFXCol.Caption = "short_desc" Then
                    oAcct.short_desc = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "icon" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.AcctIcon = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "bal1" Then

                    balance1 = oIFXRow(oIFXCol)


                ElseIf oIFXCol.Caption = "bal2" Then
                    'Roshelle 3/28/14 -- send bal2 as cashbalance for checks.
                    balance2 = oIFXRow(oIFXCol)
                ElseIf oIFXCol.caption = "chp_fee" Then
                    oAcct.oTran.Fee = oIFXRow(oIFXCol) / 100
                ElseIf oIFXCol.caption = "tranID" Then
                    oAcct.oTran.TranID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "custisdest" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.CustIsDest = 0
                    Else
                        oAcct.oTran.CustIsDest = oIFXRow(oIFXCol)
                    End If
                End If
                If (balance1 > 0 And balance2 > 0) Then
                    If (oAcct.oTran.tran_type = "CHK") Then
                        oAcct.cashBalance = balance2 / 100
                    Else
                        oAcct.cashBalance = balance1 / 100
                    End If
                End If

            Next
            If (oAcct.oTran.Amount = 0) Then
                oAcct.oTran.Amount = oAcct.oTran.AmountReq
            End If

            Dim myAcct As New clsAccount
            If (bAcct1Dep And iBlockID = oAcct.oTran.BlockId) Then
                Dim fundRow As New clsTranRow
                fundRow.ButtonText = Trim(oAcct.ButtonText)
                fundRow.StatusIcon = Trim(oAcct.AcctIcon)
                fundRow.cashBalance = oAcct.oTran.Amount
                fundRow.date_time = oAcct.oTran.CreateDate
                fundRow.short_desc = Trim(oAcct.short_desc)
                fundRow.TransactionTypeIcon = Trim(oAcct.oTran.TransactionTypeIcon)
                fundRow.TransactionTypeDescription = Trim(oAcct.oTran.TransactionTypeDescription)
                fundRow.ActID = oAcct.ActID
                fundRow.TranID = oAcct.oTran.TranID
                fundRow.BlockID = oAcct.oTran.BlockId
                fundRow.Description = oAcct.oTran.Description
                fundRow.Description2 = oAcct.oTran.Description2
                fundRow.reg_e = oAcct.oTran.reg_e
                fundRow.RunBalance = oAcct.cashBalance
                fundRow.Fee = oAcct.oTran.Fee
                fundRow.CustIsDest = oAcct.oTran.CustIsDest
                oBlockDet.oFundAct.Add(fundRow)
            Else
                arrTrans.Add(oAcct)
            End If
        Next
        Dim iCurrTran As Integer = 0
        Dim oList As New List(Of clsTranRow)
        For Each oEZAcct As clsAccount In arrTrans
            If oEZAcct.oTran.Amount <> 0 Or oEZAcct.oTran.Fee <> 0 Then
                Dim editRow As New clsTranRow
                editRow.ButtonText = Trim(oEZAcct.ButtonText)
                editRow.StatusIcon = Trim(oEZAcct.AcctIcon)
                editRow.cashBalance = oEZAcct.oTran.Amount
                editRow.date_time = oEZAcct.oTran.CreateDate
                editRow.short_desc = Trim(oEZAcct.short_desc)
                editRow.TransactionTypeIcon = Trim(oEZAcct.oTran.TransactionTypeIcon)
                editRow.TransactionTypeDescription = Trim(oEZAcct.oTran.TransactionTypeDescription)
                editRow.ActID = oEZAcct.ActID
                editRow.TranID = oEZAcct.oTran.TranID
                editRow.BlockID = oEZAcct.oTran.BlockId
                editRow.Description = oEZAcct.oTran.Description
                editRow.Description2 = oEZAcct.oTran.Description2
                editRow.reg_e = oEZAcct.oTran.reg_e
                If oEZAcct.DepositFlag Then
                    editRow.RunBalance = oEZAcct.cashBalance
                End If
                editRow.Fee = oEZAcct.oTran.Fee
                editRow.CustIsDest = oEZAcct.oTran.CustIsDest
                oBlockDet.mPaymentsTotal += editRow.cashBalance
                oBlockDet.mFeeTotal += editRow.Fee
                oBlockDet.dBlockDate = editRow.date_time
                oBlockDet.oTranRow.Add(editRow)
            End If
        Next
        oBlockDet.mBlockTotal = oBlockDet.mPaymentsTotal + oBlockDet.mFeeTotal
        dsIFX = Me.IFX_GetTransactionsByTranID(iTranID)
        For Each oIFXRow In dsIFX.Tables(0).Rows

            Dim oAcct As New clsAccount()
            Dim l As Integer = 0
            Dim bAcctmatch As Boolean = False
            Dim bAcct1Dep As Boolean = False
            Dim bAcct2match As Boolean = False
            Dim bAcct2Dep As Boolean = False
            Dim oAct1Acct As New clsAccount
            Dim oAct2Acct As New clsAccount

            oAcct.CustomerID = iCustID
            Dim bCustmatch As Boolean = False
            For Each oIFXCol In dsIFX.Tables(0).Columns
                If oIFXCol.Caption = "amount_auth" Then
                    oAcct.oTran.Amount = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "amount_req" Then
                    oAcct.oTran.AmountReq = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "tp_datetime" Then
                    oAcct.oTran.CreateDate = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pri_tran_code" Then
                    oAcct.oTran.tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "sec_tran_code" Then
                    oAcct.oTran.sec_tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_1_nbr" Then
                    oAcct.oTran.ActNbr = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_2_nbr" Then
                    oAcct.oTran.Act2Nbr = oIFXRow(oIFXCol)
                    l = 0
                    While l < arrAcct.Count
                        Dim oAccount = New clsAccount
                        oAccount = arrAcct(l).Clone(arrAcct(l).oTran)
                        'Updated by Roshelle 10/29 to compare acct numbers by decrypting
                        If oAcct.oTran.ActNbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then
                            bAcctmatch = True
                            oAct1Acct = oAccount.Clone(oAccount.oTran)
                            If oAct1Acct.DepositFlag Then
                                bAcct1Dep = True
                            End If
                        End If
                        If Int32.Parse(oAcct.oTran.Act2Nbr) = oAccount.ActID Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                            'Updated by Roshelle 10/29 to compare acct numbers by decrypting
                        ElseIf oAcct.oTran.Act2Nbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                        End If
                        l += 1
                    End While

                    If bAcctmatch And Not bAcct2match Then
                        oAcct.ButtonText = oAct1Acct.ButtonText
                        oAcct.ActID = oAct1Acct.ActID
                        oAcct.DepositFlag = oAct1Acct.DepositFlag
                        If oAcct.CustomerID <> oAct1Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct1Acct.ActNbr
                        oAcct.oTran.Amount = -oAcct.oTran.Amount
                    ElseIf bAcct2match Then
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAct2Acct.DepositFlag Then
                            oAcct.ActID = 0
                        End If
                    Else
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAcct.CustomerID <> oAct2Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        bAcct2Dep = True
                    End If

                ElseIf oIFXCol.Caption = "tranid" Then
                    oAcct.oTran.TranID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "type_code" Then
                    oAcct.oTran.tran_status = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pos_merch_nbr" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.BlockId = 0
                    Else
                        oAcct.oTran.BlockId = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "tp_reg_e" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.reg_e = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description2" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description2 = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription2" Then
                    If bAcct2match Then

                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct1icon" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct2icon" Then
                    If bAcct2match Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If
                    End If
                ElseIf oIFXCol.Caption = "short_desc" Then
                    oAcct.short_desc = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "icon" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.AcctIcon = oIFXRow(oIFXCol)
                    End If
                    'Roshelle 3/28/14 -- send bal2 as cashbalance for checks.
                ElseIf oIFXCol.Caption = "bal1" Then
                    If oAcct.oTran.tran_type <> "CHK" Then
                        oAcct.cashBalance = oIFXRow(oIFXCol)
                        oAcct.cashBalance = oAcct.cashBalance / 100
                    End If
                ElseIf oIFXCol.Caption = "bal2" Then

                    If oAcct.oTran.tran_type = "CHK" Then
                        oAcct.cashBalance = oIFXRow(oIFXCol)
                        oAcct.cashBalance = oAcct.cashBalance / 100
                    End If
                ElseIf oIFXCol.caption = "chp_fee" Then
                    oAcct.oTran.Fee = oIFXRow(oIFXCol) / 100
                ElseIf oIFXCol.Caption = "custisdest" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.CustIsDest = 0
                    Else
                        oAcct.oTran.CustIsDest = oIFXRow(oIFXCol)
                    End If
                End If
            Next
            If (oAcct.oTran.Amount = 0) Then
                oAcct.oTran.Amount = oAcct.oTran.AmountReq
            End If
            arrHeader.Add(oAcct)
        Next
        oList = New List(Of clsTranRow)
        For Each oEZAcct As clsAccount In arrHeader
            If oEZAcct.oTran.Amount <> 0 Then
                Dim editRow As New clsTranRow
                editRow.ButtonText = Trim(oEZAcct.ButtonText)
                editRow.StatusIcon = Trim(oEZAcct.AcctIcon)
                editRow.cashBalance = oEZAcct.oTran.Amount
                editRow.RunBalance = oEZAcct.cashBalance
                editRow.date_time = oEZAcct.oTran.CreateDate
                editRow.short_desc = Trim(oEZAcct.short_desc)
                editRow.TransactionTypeIcon = Trim(oEZAcct.oTran.TransactionTypeIcon)
                editRow.TransactionTypeDescription = Trim(oEZAcct.oTran.TransactionTypeDescription)
                editRow.ActID = oEZAcct.ActID
                editRow.TranID = iTranID
                editRow.BlockID = oEZAcct.oTran.BlockId
                editRow.Description = Trim(oEZAcct.oTran.Description)
                editRow.Description2 = Trim(oEZAcct.oTran.Description2)
                editRow.reg_e = Trim(oEZAcct.oTran.reg_e)
                editRow.CustID = oEZAcct.CustomerID
                editRow.ActNbr = oEZAcct.ActNbr
                editRow.CustIsDest = oEZAcct.oTran.CustIsDest
                oBlockDet.oHeaderRow.Add(editRow)
            End If
        Next
        Return oBlockDet
    End Function
   


    Public Function GetAllTransactionsbyAccount(ByVal iActID As Integer, ByVal dtFrom As String, ByVal dtTo As String) As List(Of clsTranRow)
        Dim dt As New DataTable("AllTransactions")
        Dim arrIFXAcct As New ArrayList()
        Dim dsIFX As DataSet
        Dim actNbr As String = GetActNbr(iActID)
        Dim iCustID As Integer = GetCustFromAccount(iActID)
        Dim acctDS As DataSet = GetEZCashAccountInfo(iActID)
        Dim acctType As Integer = acctDS.Tables(0).Rows(0).Item("AccountTypeID")
        If acctType = 4 Or acctType = 13 Then
            dsIFX = IFX_GetAllTransByActNbrAndDR(iActID, dtFrom, dtTo)
        Else
            dsIFX = IFX_GetAllTransByActNbrAndDR(Decrypt(actNbr), dtFrom, dtTo)
        End If

        Dim arrTrans As New ArrayList

        For Each oIFXRow In dsIFX.Tables(0).Rows
            Dim oAcct As New clsAccount()
            For Each objRow In acctDS.Tables(0).Rows
                For Each objCol In acctDS.Tables(0).Columns
                    If objCol.Caption = "ActNbr" Then
                        oAcct.ActNbr = objRow(objCol)
                    ElseIf objCol.Caption = "DepositFlag" Then
                        oAcct.DepositFlag = objRow(objCol)
                    ElseIf objCol.Caption = "ButtonText" Then
                        oAcct.ButtonText = objRow(objCol)
                    ElseIf objCol.Caption = "ActID" Then
                        oAcct.ActID = objRow(objCol)
                    ElseIf objCol.Caption = "CustomerID" Then
                        oAcct.CustomerID = objRow(objCol)

                    End If
                Next
            Next
            Dim l As Integer = 0
            Dim bAcctmatch As Boolean = False
            Dim bAcct2match As Boolean = False
            Dim oAct1Acct As New clsAccount
            Dim oAct2Acct As New clsAccount

            Dim bCustmatch As Boolean = False
            For Each oIFXCol In dsIFX.Tables(0).Columns
                If oIFXCol.Caption = "amount_auth" Then
                    oAcct.oTran.Amount = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "pos_batch_nbr" Then
                    oAcct.CustomerID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "tp_datetime" Then
                    oAcct.oTran.CreateDate = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pri_tran_code" Then
                    oAcct.oTran.tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "sec_tran_code" Then
                    oAcct.oTran.sec_tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_1_nbr" Then
                    oAcct.oTran.ActNbr = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_2_nbr" Then
                    oAcct.oTran.Act2Nbr = oIFXRow(oIFXCol)
                    If oAcct.oTran.ActNbr = leftPad(Decrypt(actNbr), 18, "0") Then
                        bAcctmatch = True
                        oAcct.oTran.ActNbr = leftPad(Decrypt(actNbr), 18, "0")
                    End If
                    If oAcct.oTran.Act2Nbr = leftPad(Decrypt(actNbr), 18, "0") Then
                        bAcct2match = True
                    End If
                    If bAcctmatch And Not bAcct2match Then
                        If oAcct.CustomerID <> oAct1Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.oTran.Amount = -oAcct.oTran.Amount
                    Else
                        If oAcct.CustomerID <> oAct2Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                    End If

                ElseIf oIFXCol.Caption = "tranid" Then
                    oAcct.oTran.TranID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "type_code" Then
                    oAcct.oTran.tran_status = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "chp_fee" Then
                    oAcct.oTran.Fee = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pos_merch_nbr" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.BlockId = 0
                    Else
                        oAcct.oTran.BlockId = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "tp_reg_e" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.reg_e = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description2" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description2 = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "transactiontypedescription2" Then
                    If bAcct2match Then

                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct1icon" Then
                    If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If

                    End If
                ElseIf oIFXCol.Caption = "acct2icon" Then
                    If bAcct2match Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                        End If
                    End If
                ElseIf oIFXCol.Caption = "short_desc" Then
                    oAcct.short_desc = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "icon" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.AcctIcon = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "bal1" Then
                    oAcct.cashBalance = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "chp_fee" Then
                    oAcct.oTran.Fee = oIFXRow(oIFXCol) / 100
                End If
            Next
            Dim myAcct As New clsAccount
            If oAcct.oTran.Amount <> 0 Then
                arrTrans.Add(oAcct)
            Else
                Continue For
            End If
            If bAcctmatch Then
                oAcct.oTran.Amount = oAcct.oTran.Amount + myAcct.oTran.Fee
            End If
        Next
        Dim oList As New List(Of clsTranRow)
        For Each oEZAcct As clsAccount In arrTrans
            If oEZAcct.oTran.Amount <> 0 Then
                Dim editRow As New clsTranRow
                editRow.ButtonText = Trim(oEZAcct.ButtonText)
                editRow.StatusIcon = Trim(oEZAcct.AcctIcon)
                editRow.cashBalance = oEZAcct.oTran.Amount
                editRow.RunBalance = oEZAcct.cashBalance
                editRow.date_time = oEZAcct.oTran.CreateDate
                editRow.short_desc = Trim(oEZAcct.short_desc)
                editRow.TransactionTypeIcon = Trim(oEZAcct.oTran.TransactionTypeIcon)
                editRow.TransactionTypeDescription = Trim(oEZAcct.oTran.TransactionTypeDescription)
                editRow.ActID = oEZAcct.ActID
                editRow.TranID = oEZAcct.oTran.TranID
                editRow.BlockID = oEZAcct.oTran.BlockId
                editRow.Description = oEZAcct.oTran.Description.Trim()
                editRow.Description2 = oEZAcct.oTran.Description2.Trim()
                editRow.reg_e = oEZAcct.oTran.reg_e.Trim()
                oList.Add(editRow)
            End If
        Next
        Return oList
    End Function
    Public Function GetWebCompAllTypes(ByVal iCustID As Integer, ByVal oAcct As clsAccount) As ArrayList
        Dim k As Integer = 0
        Dim arrWebComp As New ArrayList
        Dim arrRes As New ArrayList
        Dim dsWC2 As DataSet = GetWebComponentInfo(oAcct.oTran.ActNbr, oAcct.oTran.tran_type, oAcct.oTran.sec_tran_type, oAcct.oTran.tran_status, iCustID)
        For Each oezRow As DataRow In dsWC2.Tables(0).Rows
            For Each ezcol In dsWC2.Tables(0).Columns
                If ezcol.Caption = "ButtonText" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.ButtonText = ""
                    Else
                        oAcct.ButtonText = oezRow(ezcol)
                    End If
                End If
                If ezcol.Caption = "TransactionTypeDescription" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeDescription = ""
                    Else
                        oAcct.oTran.TransactionTypeDescription = oezRow(ezcol)
                    End If
                ElseIf ezcol.Caption = "Acct1Icon" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeIcon = ""
                    Else
                        oAcct.oTran.TransactionTypeIcon = oezRow(ezcol)
                    End If

                End If
                If ezcol.Caption = "TransactionTypeDescription2" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeDescription2 = ""
                    Else
                        oAcct.oTran.TransactionTypeDescription2 = oezRow(ezcol)
                    End If
                ElseIf ezcol.Caption = "Acct2Icon" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeIcon2 = ""
                    Else
                        oAcct.oTran.TransactionTypeIcon2 = oezRow(ezcol)
                    End If
                End If
                If ezcol.Caption = "short_desc" Then
                    oAcct.short_desc = oezRow(ezcol)
                ElseIf ezcol.Caption = "icon" Then
                    If IsDBNull(oezRow(ezcol)) Then
                    Else
                        oAcct.AcctIcon = oezRow(ezcol)
                    End If
                ElseIf ezcol.Caption = "ActID" Then
                    oAcct.ActID = oezRow(ezcol)
                End If
            Next
            arrWebComp.Add(oAcct)
        Next
        Dim dsWC As DataSet = GetWebComponentInfo(oAcct.oTran.Act2Nbr, oAcct.oTran.tran_type, oAcct.oTran.sec_tran_type, oAcct.oTran.tran_status, iCustID)
        For Each oezRow As DataRow In dsWC2.Tables(0).Rows
            For Each ezcol In dsWC2.Tables(0).Columns
                If ezcol.Caption = "ButtonText" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.ButtonText = ""
                    Else
                        oAcct.ButtonText = oezRow(ezcol)
                    End If
                End If
                If ezcol.Caption = "TransactionTypeDescription2" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeDescription = ""
                    Else
                        oAcct.oTran.TransactionTypeDescription = oezRow(ezcol)
                    End If
                ElseIf ezcol.Caption = "Acct2Icon" Then
                    If IsDBNull(oezRow(ezcol)) Then
                        oAcct.oTran.TransactionTypeIcon = ""
                    Else
                        oAcct.oTran.TransactionTypeIcon = oezRow(ezcol)
                    End If

                End If
                If ezcol.Caption = "short_desc" Then
                    oAcct.short_desc = oezRow(ezcol)
                ElseIf ezcol.Caption = "icon" Then
                    If IsDBNull(oezRow(ezcol)) Then
                    Else
                        oAcct.AcctIcon = oezRow(ezcol)
                    End If
                ElseIf ezcol.Caption = "ActID" Then
                    oAcct.ActID = oezRow(ezcol)
                End If
            Next
            arrWebComp.Add(oAcct)
        Next
        Return arrWebComp
    End Function
    Public Function GetAllTransactionsDS(ByVal iCustID As Integer, ByVal acctDs As DataSet, ByVal dtFrom As String, ByVal dtTo As String) As List(Of clsTranRow)
        'Dim acctDs As DataSet = GetEZCashAccounts(iCustID)
        ' oChkService.Trace("getalltransactionsDS")
        Dim dt As New DataTable("AllTransactions")
        Dim ifxCol As New DataColumn
        Dim arrIFXAcct As New ArrayList()
        Dim dsIFX As New DataSet
        Dim arrTrans As New ArrayList()
        Dim arrDep As New ArrayList

        Dim dsDep As DataSet = GetEZCashDepositAccounts(iCustID)
        'get accounts where deposit flag is 1
        For Each objRow In dsDep.Tables(0).Rows
            Dim aAcct As New clsAccount
            For Each objCol In dsDep.Tables(0).Columns
                'oChkService.Trace(objCol.Caption.ToString + objRow(objCol).ToString)

                If objCol.Caption = "ActNbr" Then
                    aAcct.ActNbr = objRow(objCol)
                ElseIf objCol.Caption = "DepositFlag" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.DepositFlag = 0
                    Else
                        aAcct.DepositFlag = objRow(objCol)
                    End If

                ElseIf objCol.Caption = "ButtonText" Then
                    aAcct.ButtonText = objRow(objCol)
                ElseIf objCol.Caption = "ActID" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.ActID = 0
                    Else
                        aAcct.ActID = objRow(objCol)
                    End If

                ElseIf objCol.Caption = "CustomerID" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.CustomerID = 0
                    Else
                        aAcct.CustomerID = objRow(objCol)
                    End If
                End If
            Next
            arrDep.Add(aAcct)
        Next
        'oChkService.Trace("arrDep size:" + arrDep.Count.ToString)

        'get all accounts for the cust
        Dim arrAcct As New ArrayList
        For Each objRow In acctDs.Tables(0).Rows
            Dim aAcct As New clsAccount
            For Each objCol In acctDs.Tables(0).Columns
                'oChkService.Trace(objCol.Caption.ToString + " " + objRow(objCol).ToString)
                If objCol.Caption = "ActNbr" Then
                    aAcct.ActNbr = objRow(objCol)
                ElseIf objCol.Caption = "DepositFlag" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.DepositFlag = 0
                    Else
                        aAcct.DepositFlag = objRow(objCol)
                    End If
                ElseIf objCol.Caption = "ButtonText" Then
                    aAcct.ButtonText = objRow(objCol)
                ElseIf objCol.Caption = "ActID" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.ActID = 0
                    Else
                        aAcct.ActID = objRow(objCol)
                    End If
                ElseIf objCol.Caption = "CustomerID" Then
                    If IsDBNull(objRow(objCol)) Then
                        aAcct.CustomerID = 0
                    Else
                        aAcct.CustomerID = objRow(objCol)
                    End If
                End If
            Next
            arrAcct.Add(aAcct)
            'oChkService.Trace("arrAcct size:" + arrAcct.Count.ToString)
        Next
        'get every transaction for all accounts


        dsIFX = IFX_GetAllTransByCustID(iCustID, arrDep, GetEZCashCards(iCustID), dtFrom, dtTo)
        For Each oIFXRow In dsIFX.Tables(0).Rows
            Dim oAcct As New clsAccount()
            Dim l As Integer = 0
            Dim bAcctmatch As Boolean = False
            Dim bAcct2match As Boolean = False
            Dim bAcct1Dep As Boolean = False
            Dim bAcct2Dep As Boolean = False
            Dim oAct1Acct As New clsAccount
            Dim oAct2Acct As New clsAccount

            For Each oIFXCol In dsIFX.Tables(0).Columns
                'oChkService.Trace(oIFXCol.Caption + ": " + oIFXRow(oIFXCol).ToString)

                If oIFXCol.Caption = "amount_req" Then
                    oAcct.oTran.AmountReq = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "amount_auth" Then
                    oAcct.oTran.Amount = Double.Parse(oIFXRow(oIFXCol) / 100)
                ElseIf oIFXCol.Caption = "pos_batch_nbr" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.CustomerID = 0
                    Else
                        oAcct.CustomerID = oIFXRow(oIFXCol)
                    End If
                    'oAcct.CustomerID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "tp_datetime" Then
                    oAcct.oTran.CreateDate = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "pri_tran_code" Then
                    oAcct.oTran.tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "sec_tran_code" Then
                    oAcct.oTran.sec_tran_type = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_1_nbr" Then
                    oAcct.oTran.ActNbr = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "acct_2_nbr" Then
                    oAcct.oTran.Act2Nbr = oIFXRow(oIFXCol)
                    l = 0
                    'compare every account the customer has to the accounts used in the transaction row
                    While l < arrAcct.Count
                        Dim oAccount = New clsAccount
                        oAccount = arrAcct(l).Clone(arrAcct(l).oTran)
                        ' Compare the 'From' account number to the acct number (which is ecrypted in EZCash's Accounts DB)
                        ' Account num is EZCash is 9, but is 18 in SWX, so pad EZCash acct
                        If oAcct.oTran.ActNbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then

                            bAcctmatch = True
                            'oAct1Acct is a clone of the Account row in EZCash.Accounts table
                            oAct1Acct = oAccount.Clone(oAccount.oTran)
                            If oAct1Acct.DepositFlag Then
                                ' baAcct1Dep = The Account is the 'from' and the depositflag is true
                                bAcct1Dep = True
                            End If
                        End If
                        '   if the 'To' Account number is the account ID of the row in Accounts table
                        If Int32.Parse(oAcct.oTran.Act2Nbr) = oAccount.ActID Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                            ' if the 'To' Account number encrypted matches the Acct number from Accounts table 
                        ElseIf oAcct.oTran.Act2Nbr = leftPad(Decrypt(oAccount.ActNbr), 18, "0") Then
                            If oAccount.DepositFlag Then
                                bAcct2Dep = True
                            End If
                            bAcct2match = True
                            oAct2Acct = oAccount.Clone(oAccount.oTran)
                        End If
                        l += 1
                    End While
                    ' match on the From account
                    If bAcctmatch And Not bAcct2match Then
                        oAcct.ButtonText = oAct1Acct.ButtonText
                        oAcct.ActID = oAct1Acct.ActID
                        oAcct.DepositFlag = oAct1Acct.DepositFlag
                        If oAcct.CustomerID <> oAct1Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct1Acct.ActNbr
                        'money is being moved from so it is neg
                        oAcct.oTran.Amount = -oAcct.oTran.Amount
                        ' match on the To account
                    ElseIf bAcct2match Then
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAct2Acct.DepositFlag Then
                            oAcct.ActID = 0
                        End If
                    Else
                        oAcct.ButtonText = oAct2Acct.ButtonText
                        oAcct.ActID = oAct2Acct.ActID
                        oAcct.DepositFlag = oAct2Acct.DepositFlag
                        If oAcct.CustomerID <> oAct2Acct.CustomerID Then
                            oAcct.ActID = 0
                        End If
                        oAcct.ActNbr = oAct2Acct.ActNbr
                        bAcct2Dep = True
                    End If
                    

                ElseIf oIFXCol.Caption = "tranid" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.TranID = 0
                    Else
                        oAcct.oTran.TranID = oIFXRow(oIFXCol)
                    End If
                    'oAcct.oTran.TranID = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "type_code" Then
                    oAcct.oTran.tran_status = oIFXRow(oIFXCol)
                    'Roshelle 5/12/14 -- using different field for BlockID now.
                ElseIf oIFXCol.Caption = "pos_merch_nbr" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                        oAcct.oTran.BlockId = 0
                    Else
                        oAcct.oTran.BlockId = oIFXRow(oIFXCol)
                    End If
                ElseIf oIFXCol.Caption = "description" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.Description = oIFXRow(oIFXCol)
                    End If

                ElseIf oIFXCol.Caption = "tp_reg_e" Then
                    If IsDBNull(oIFXRow(oIFXCol)) Then
                    Else
                        oAcct.oTran.reg_e = oIFXRow(oIFXCol)
                    End If

                ElseIf oIFXCol.Caption = "description2" Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.oTran.Description2 = oIFXRow(oIFXCol)
                        End If

                ElseIf oIFXCol.Caption = "transactiontypedescription" Then
                        If bAcctmatch Or (Not bAcctmatch And Not bAcct2Dep) Then

                            If IsDBNull(oIFXRow(oIFXCol)) Then
                            Else
                                oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                            End If

                        End If
                ElseIf oIFXCol.Caption = "transactiontypedescription2" Then
                        If bAcct2Dep Then
                            If IsDBNull(oIFXRow(oIFXCol)) Then
                            Else
                                oAcct.oTran.TransactionTypeDescription = oIFXRow(oIFXCol)
                            End If

                        End If
                ElseIf oIFXCol.Caption = "acct1icon" Then
                        If bAcctmatch Or (Not bAcctmatch And Not bAcct2Dep) Then
                            If IsDBNull(oIFXRow(oIFXCol)) Then
                            Else
                                oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                            End If
                        End If

                ElseIf oIFXCol.Caption = "acct2icon" Then
                        If bAcct2Dep Then
                            If IsDBNull(oIFXRow(oIFXCol)) Then
                            Else
                                oAcct.oTran.TransactionTypeIcon = oIFXRow(oIFXCol)
                            End If

                        End If
                ElseIf oIFXCol.Caption = "short_desc" Then
                        oAcct.short_desc = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "icon" Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                        Else
                            oAcct.AcctIcon = oIFXRow(oIFXCol)
                        End If
                ElseIf oIFXCol.Caption = "bal1" Then
                        oAcct.cashBalance = oIFXRow(oIFXCol)
                ElseIf oIFXCol.Caption = "chp_fee" Then
                        oAcct.oTran.Fee = Double.Parse((oIFXRow(oIFXCol) / 100))
                        If bAcctmatch Or (Not bAcctmatch And Not bAcct2match) Then
                            oAcct.oTran.Amount = oAcct.oTran.Amount + oAcct.oTran.Fee
                        End If
                ElseIf oIFXCol.Caption = "custisdest" Then
                        If IsDBNull(oIFXRow(oIFXCol)) Then
                            oAcct.IsCustDest = 0
                        Else
                            oAcct.IsCustDest = oIFXRow(oIFXCol)
                        End If
                    End If

            Next
            If oAcct.oTran.Amount = 0 Then
                oAcct.oTran.Amount = oAcct.oTran.AmountReq
            End If
            Dim myAcct As New clsAccount
            If oAcct.oTran.BlockId = 0 Then
                oAcct.ActID = 0
            End If
            If oAcct.oTran.Amount <> 0 Then
                arrTrans.Add(oAcct)
            Else
                Continue For
            End If


        Next
        Dim oList As New List(Of clsTranRow)
        For Each oEZAcct As clsAccount In arrTrans
            If oEZAcct.oTran.Amount <> 0 Then
                Dim editRow As New clsTranRow
                editRow.ButtonText = Trim(oEZAcct.ButtonText)
                editRow.StatusIcon = Trim(oEZAcct.AcctIcon)
                editRow.cashBalance = oEZAcct.oTran.Amount
                editRow.RunBalance = oEZAcct.cashBalance
                editRow.date_time = oEZAcct.oTran.CreateDate
                editRow.short_desc = Trim(oEZAcct.short_desc)
                editRow.TransactionTypeIcon = Trim(oEZAcct.oTran.TransactionTypeIcon)
                editRow.TransactionTypeDescription = Trim(oEZAcct.oTran.TransactionTypeDescription)
                editRow.ActID = oEZAcct.ActID
                editRow.TranID = oEZAcct.oTran.TranID
                editRow.BlockID = oEZAcct.oTran.BlockId
                editRow.custisdest = oEZAcct.IsCustDest
               
                editRow.Description = oEZAcct.oTran.Description.Trim()
                editRow.Description2 = oEZAcct.oTran.Description2.Trim()
                editRow.reg_e = oEZAcct.oTran.reg_e.Trim()
                editRow.CustID = oEZAcct.CustomerID
                editRow.ActNbr = oEZAcct.ActNbr
                oList.Add(editRow)
            End If
        Next
        Return oList
    End Function
    Public Function MatchPreregData(ByVal Barcode As Integer) As DataSet

        Dim sqlCmd As New SqlCommand("Select * from CustomerPhotoIDs where Barcode = " + Barcode.ToString(), oConn.sqlConn)

        sqlCmd.CommandType = CommandType.Text
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        Return ds
    End Function
    Public Function IFX_GetAccountInfo(ByVal ActNbr As String) As DataSet
        Dim sqlCmd As New IfxCommand("select first 1 avail_bal/100 as cashBalance " + _
        "FROM(swx.standby_auth) " + _
        "WHERE account_nbr = '" + ActNbr + "' " + _
        "group by avail_bal " + _
        "having(avail_bal > 0) ", oConn.ifxConn)

        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        Return ds

    End Function
    Public Function IFX_RetrieveBarcodeForPan(ByVal Bin As String, ByVal CardHolder As String) As Integer
        Dim barcode As String = ""
        oConn.Connect(True)
        Dim sqlCmd As New IfxCommand("select barcode from cardholder where (cardholder = " + CardHolder + " And Bin = " + Bin + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader = sqlCmd.ExecuteReader()
        If dataReader.HasRows Then
            While dataReader.Read
                barcode = dataReader(0)
            End While
            Return barcode
        Else
            Return 0
        End If
        oConn.Close()


    End Function
    Public Function IFX_RetrieveCompanyNumberForPan(ByVal Bin As String, ByVal CardHolder As String) As Integer
        Dim branch_nbr As Integer
        oConn.Connect(True)
        Dim sqlCmd As New IfxCommand("select branch_nbr from cardholder where (cardholder = " + CardHolder + " And Bin = " + Bin + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim dataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            branch_nbr = dataReader(0)
        End While
        oConn.Close()
        Return branch_nbr
    End Function
    Public Function IFX_InsertMessage(ByVal ToEmail As String, ByVal ToPhone As String, _
                                      ByVal MsgType As Integer, ByVal Subject As String, _
                                      ByVal Message As String, ByVal seq_nbr As Integer) As Boolean
        oConn.Connect(True)
        Dim sqlCmd As New IfxCommand("insert into tgs_out_msgs(msg_type,msg_body,to_email,to_phone,email_subject,processed,err_flag,seq_nbr)" + _
                                     " values( " + MsgType.ToString + ",'" + Message + "','" + ToEmail + "','" + ToPhone + "','" + Subject + _
                                     "',0,0," + seq_nbr.ToString + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text

        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True

    End Function
    Public Function GetEZCashAccounts(ByVal iCustID As Integer) As DataSet
        'Gets all Accounts associated with that CustID
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectAccounts", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function GetEZCashAccountInfo(ByVal iActID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectAccountInfo", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("ActID", iActID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashInactiveAccounts(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectInactiveAccounts", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashDepositAccounts(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectDepositAccounts", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashPayeeAccounts(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectPayeeAccounts", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashTransactions(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectTransactions", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashTransactionsByAcct(ByVal iActID As Integer) As DataSet
        If oConn.sqlConn.State = ConnectionState.Closed Then
            oConn.Connect(False)
        End If
        Dim sqlCmd As New SqlCommand("sp_SelectTransactionsByActID", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iActID", iActID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashTransactionsByBlockAndCust(ByVal iBlockID As Integer, ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectTransactionsByBlockIDAndCustID", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iBlockID", iBlockID)
        sqlCmd.Parameters.Add(parm1)
        Dim parm2 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetSrcTransactionsByAcct(ByVal iActID As Integer, ByVal strActNbr As String) As DataSet
        Dim ds As New DataSet
        oConn.Connect(True)
        Dim strCard As String = Decrypt(GetCard(iActID))
        Dim strBin As String = strCard.Substring(0, 6)
        Dim strCH As String = strCard.Substring(6, 13)
        Dim sqlCmd As New IfxCommand("SELECT tp_reg_e, desc1 as description, desc2 as description2, type_code, pos_merch_nbr, acct_1_nbr,acct_2_nbr,tp_seq as tranID, bin, cardholder, amount_auth, tp_datetime, pos_batch_nbr, pri_tran_code, sec_tran_code " + _
        "FROM(swx.log_record t1) " + _
        "WHERE (t1.bin = " + strBin + " and t1.cardholder= " + strCH + " and tp_inst_nbr > 0) and acct_1_nbr = " + strActNbr, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function IFX_GetDestTransactionsByAcct(ByVal iActID As Integer, ByVal strActNbr As String) As DataSet
        Dim ds As New DataSet
        oConn.Connect(True)
        Dim strCard As String = Decrypt(GetCard(iActID))
        Dim strBin As String = strCard.Substring(0, 6)
        Dim strCH As String = strCard.Substring(6, 13)
        Dim sqlCmd As New IfxCommand("SELECT tp_reg_e, desc1 as description, desc2 as description2,type_code, pos_merch_nbr, acct_1_nbr,acct_2_nbr,tp_seq as tranID, bin, cardholder, amount_auth, tp_datetime, pos_batch_nbr, pri_tran_code, sec_tran_code " + _
        "FROM(swx.log_record t1) " + _
        "WHERE (t1.bin = " + strBin + " and t1.cardholder= " + strCH + " and tp_inst_nbr > 0) and acct_2_nbr = " + strActNbr, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetTransactionsByBlock(ByVal iBlockID As Integer, ByVal iCustID As Integer) As DataSet
        Dim ds As New DataSet

        oConn.Connect(True)
        'Roshelle 3/28/14 -- Added bal 2
        Dim sqlCmd As New IfxCommand("SELECT amount_req,pri_tran_code, sec_tran_code,pos_merch_nbr,bal_1 as bal1,bal_2 as bal2,tp_seq as tranid,amount_auth,acct_1_nbr,acct_2_nbr,tp_datetime,pos_merch_nbr, tp_reg_e, desc1 as description, desc2 as description2,type_code, chp_fee,bin,  cardholder,   pos_batch_nbr,  " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc " + _
                               "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
        "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and (t1.pos_merch_nbr = " + iBlockID.ToString() + " and t1.pos_batch_nbr = " + iCustID.ToString() + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetTransactionsByTranID(ByVal iTranID As Integer) As DataSet
        Dim ds As New DataSet
        oConn.Connect(True)
        Dim sqlCmd As New IfxCommand("SELECT tp_reg_e, desc1 as description, desc2 as description2,type_code,  bal_1,bal_2,chp_fee,acct_1_nbr,acct_2_nbr,tp_seq as tranID, bin, cardholder, amount_auth, tp_datetime, pos_batch_nbr, pri_tran_code, sec_tran_code, " + _
 "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc " + _
                          "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
        "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and (t1.tp_seq = " + iTranID.ToString() + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetFeeByTranID(ByVal iTranID As Integer) As DataSet
        Dim ds As New DataSet
        oConn.Connect(True)
        Dim sqlCmd As New IfxCommand("SELECT chp_fee " + _
        "FROM(swx.log_record t1) " + _
        "WHERE (t1.tp_seq = " + iTranID.ToString() + ")", oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function GetEZCashCards(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCustCards", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)

        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetAllTransByCustID(ByVal iCustID As Integer, ByVal ActNbrDS As ArrayList, ByVal CardNbrDS As DataSet, _
                                            ByVal dtFrom As String, ByVal dtTo As String) As DataSet
        Dim ds As New DataSet

        Dim arrAcct As New ArrayList
        'oChkService.Trace("start getalltransbycustid")

        For Each oRow In CardNbrDS.Tables(0).Rows
            For Each oCol In CardNbrDS.Tables(0).Columns
                arrAcct.Add(CType(Decrypt(oRow(oCol)), String))
            Next
        Next
        Dim strBin As String = ""
        Dim strCardHolder As String = ""
        Dim strQueryBin As String = ""
        For Each CardNbr As String In arrAcct
            If strQueryBin <> "" Then
                strQueryBin += " or (t1.bin = "
            Else
                strQueryBin = "(t1.bin="
            End If
            strQueryBin += Int32.Parse(CardNbr.Substring(0, 7)).ToString() + " and t1.cardholder= " + Int32.Parse(CardNbr.Substring(8, 8)).ToString() + " and t1.tp_inst_nbr = 0 and t1.pri_tran_code <> 'PUT' )"


        Next

        If strQueryBin.Length > 0 Then
            strQueryBin = strQueryBin.Substring(0, strQueryBin.Length - 1) + ")"
        End If
        'oChkService.Trace(strQueryBin)
        Dim strQueryAct2 As String = ""


        For Each AcctNbr As clsAccount In ActNbrDS
            If strQueryAct2 <> "" Then
                strQueryAct2 += " or (t1.acct_2_nbr = "
            Else
                strQueryAct2 += " (t1.acct_2_nbr = "
            End If
            strQueryAct2 += Decrypt(AcctNbr.ActNbr).PadLeft(18, "0") + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT' )"

        Next
        'oChkService.Trace(strQueryAct2)
        'distinct case when count(pos_merch_nbr) > 1 then pos_merch_nbr else 0 end as pos_merch_nbr
        'select case when count(pos_merch_nbr) < 2 then 0 else pos_merch_nbr end as pos_merch_nbr
        oConn.Connect(True)

        Dim strSQL As String = "SELECT  amount_req,amount_auth,pos_batch_nbr,acct_1_nbr,acct_2_nbr,chp_fee,tp_datetime,pos_merch_nbr, tp_reg_e, desc1 as description, desc2 as description2, type_code, chp_fee,bin,  cardholder,   pri_tran_code, sec_tran_code, " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc,tp_seq as tranid " + _
                               "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
                               "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and ((t1.pos_batch_nbr = " + iCustID.ToString + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT')"
        If strQueryBin <> "" Then
            strSQL += " or " + strQueryBin + " or " + strQueryAct2 + ")"
        Else
            strSQL += ")"
        End If
        If Not (IsDBNull(dtFrom)) And Not (IsDBNull(dtTo)) Then
            strSQL += " and (tran_datetime >= Date('" + dtFrom + "') and tran_dateTime <= Date('" + dtTo + "') + 1)"
        Else
            strSQL += " and (tran_datetime >= Date('" + dtFrom + "') + 1)"
        End If

        strSQL += " order by tran_datetime desc"
        'oChkService.Trace(strSQL)

        Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetTransByTranD(ByVal iTranID As Integer)
        Dim ds As New DataSet
        oConn.Connect(True)
        Dim strSQL As String = "SELECT  amount_req,amount_auth,pos_batch_nbr,acct_1_nbr,acct_2_nbr,chp_fee,tp_datetime,pos_merch_nbr, tp_reg_e, desc1 as description,desc2 as description2, type_code, chp_fee,bin,  cardholder,   pri_tran_code, sec_tran_code, " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc " + _
                               "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
                               "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and ((t1.tp_seq = " + iTranID.ToString + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT')"



        Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function IFX_GetFeeWebComp() As DataSet
        oConn.Connect(True)
        Dim strSQL As String = "SELECT primarytrancode, secondarytrancode, " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2 " + _
                               "FROM swx.tgs_transactiontypes t2  " + _
                               "WHERE (t2.secondarytrancode = 'FEE')"
        Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    Public Function IFX_GetAllTransByActNbrAndDR(ByVal actNbr As String, ByVal dtFrom As String, ByVal dtTo As String) As DataSet
        Dim ds As New DataSet
        Dim strQueryAct2 As String = ""
        If strQueryAct2 <> "" Then
            strQueryAct2 += " or (t1.acct_2_nbr = "
        Else
            strQueryAct2 += " (t1.acct_2_nbr = "
        End If
        strQueryAct2 += actNbr.PadLeft(18, "0") + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT' )"

        'distinct case when count(pos_merch_nbr) > 1 then pos_merch_nbr else 0 end as pos_merch_nbr
        'select case when count(pos_merch_nbr) < 2 then 0 else pos_merch_nbr end as pos_merch_nbr
        oConn.Connect(True)
        Dim strSQL As String = "SELECT    pos_batch_nbr,chp_fee,amount_auth,acct_1_nbr,acct_2_nbr,tp_datetime,pos_merch_nbr, tp_reg_e, desc1 as description, desc2 as description2, type_code, chp_fee,bin,  cardholder, pri_tran_code, sec_tran_code, " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc " + _
                               "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
                               "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and ("
        strSQL += strQueryAct2 + ")"
        If Not (IsDBNull(dtFrom)) And Not (IsDBNull(dtTo)) Then
            strSQL += " and (tran_datetime >= Date('" + dtFrom + "') and tran_dateTime <= Date('" + dtTo + "') + 1)"
        Else
            strSQL += " and (tran_datetime >= Date('" + dtFrom + "') + 1)"
        End If

        Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function
    'Public Function IFX_GetAllTransByCustIDAnd(ByVal iCustID As Integer, ByVal ActNbrDS As ArrayList, ByVal CardNbrDS As DataSet, _
    '                                       ByVal dtFrom As String, ByVal dtTo As String) As DataSet
    '    Dim ds As New DataSet

    '    Dim arrAcct As New ArrayList
    '    For Each oRow In CardNbrDS.Tables(0).Rows
    '        For Each oCol In CardNbrDS.Tables(0).Columns

    '            arrAcct.Add(CType(Decrypt(oRow(oCol)), String))

    '        Next
    '    Next
    '    Dim strBin As String = ""
    '    Dim strCardHolder As String = ""
    '    Dim strQueryBin As String = ""
    '    For Each CardNbr As String In arrAcct
    '        If strQueryBin <> "" Then
    '            strQueryBin += " or (t1.bin = "
    '        Else
    '            strQueryBin = "(t1.bin="
    '        End If
    '        strQueryBin += Int32.Parse(CardNbr.Substring(0, 7)).ToString() + " and t1.cardholder= " + Int32.Parse(CardNbr.Substring(8, 8)).ToString() + " and t1.tp_inst_nbr = 0 and t1.pri_tran_code <> 'PUT' )"

    '    Next
    '    If strQueryBin.Length > 0 Then
    '        strQueryBin = strQueryBin.Substring(0, strQueryBin.Length - 1) + ")"
    '    End If

    '    Dim strQueryAct2 As String = ""
    '    For Each AcctNbr As clsAccount In ActNbrDS
    '        If strQueryAct2 <> "" Then
    '            strQueryAct2 += " or (t1.acct_2_nbr = "
    '        Else
    '            strQueryAct2 += " (t1.acct_2_nbr = "
    '        End If
    '        strQueryAct2 += Decrypt(AcctNbr.ActNbr).PadLeft(18, "0") + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT' )"
    '    Next
    '    'distinct case when count(pos_merch_nbr) > 1 then pos_merch_nbr else 0 end as pos_merch_nbr
    '    'select case when count(pos_merch_nbr) < 2 then 0 else pos_merch_nbr end as pos_merch_nbr
    '    oConn.Connect(True)
    '    Dim strSQL As String = "SELECT  amount_req,amount_auth,pos_batch_nbr,acct_1_nbr,acct_2_nbr,chp_fee,tp_datetime,pos_merch_nbr, tp_reg_e as description,type_code, chp_fee,bin,  cardholder,   pri_tran_code, sec_tran_code, " + _
    '                           "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc,tp_seq as tranid " + _
    '                           "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
    '                           "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and ((t1.pos_batch_nbr = " + iCustID.ToString + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT')"
    '    If strQueryBin <> "" Then
    '        strSQL += " or " + strQueryBin + " or " + strQueryAct2 + ")"
    '    Else
    '        strSQL += ")"
    '    End If
    '    If Not (IsDBNull(dtFrom)) And Not (IsDBNull(dtTo)) Then
    '        strSQL += " and (tran_datetime >= Date('" + dtFrom + "') and tran_dateTime <= Date('" + dtTo + "') + 1)"
    '    Else
    '        strSQL += " and (tran_datetime >= Date('" + dtFrom + "') + 1)"
    '    End If



    '    Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
    '    sqlCmd.CommandType = CommandType.Text
    '    Dim da As New IfxDataAdapter
    '    da.SelectCommand = sqlCmd
    '    da.Fill(ds)
    '    oConn.Close()
    '    Return ds
    'End Function


    Public Function IFX_GetAllTransByActNbr(ByVal actNbr As String) As DataSet
        Dim ds As New DataSet
        Dim strQueryAct2 As String = ""
        If strQueryAct2 <> "" Then
            strQueryAct2 += " or (t1.acct_2_nbr = "
        Else
            strQueryAct2 += " (t1.acct_2_nbr = "
        End If
        strQueryAct2 += actNbr.PadLeft(18, "0") + " and t1.tp_inst_nbr = 1 and t1.pri_tran_code <> 'PUT' )"

        'distinct case when count(pos_merch_nbr) > 1 then pos_merch_nbr else 0 end as pos_merch_nbr
        'select case when count(pos_merch_nbr) < 2 then 0 else pos_merch_nbr end as pos_merch_nbr
        oConn.Connect(True)
        Dim strSQL As String = "SELECT    pos_batch_nbr,chp_fee,amount_auth,acct_1_nbr,acct_2_nbr,tp_datetime, pos_merch_nbr, tp_reg_e, desc1 as description, desc2 as description2, type_code, chp_fee,bin,  cardholder, pri_tran_code, sec_tran_code, " + _
                               "t2.acct1icon, t2.acct2icon, t2.custisdest, t2.description as TransactionTypeDescription, t2.description2 as TransactionTypeDescription2, t3.icon, t3.short_desc " + _
                               "FROM swx.log_record t1, swx.tgs_transactiontypes t2, swx.tgs_tran_status_desc t3  " + _
                               "WHERE (t2.primarytrancode = trim(t1.pri_tran_code) and t2.secondarytrancode = trim(t1.sec_tran_code) and t3.tran_status = t1.type_code) and ("
        strSQL += strQueryAct2 + ")"
        Dim sqlCmd As New IfxCommand(strSQL, oConn.ifxConn)
        sqlCmd.CommandType = CommandType.Text
        Dim da As New IfxDataAdapter
        da.SelectCommand = sqlCmd
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function GetHeldEZCashTrans(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetHeldTransactions", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        sqlCmd.Parameters.Add(parm1)

        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function

    Public Function GetWebComponentInfo(ByVal ActNbr As String, ByVal PriTranCode As String, ByVal SecTranCode As String, ByVal Status As Integer, ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SelectWebComponents", oConn.sqlConn)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim parm1 As New SqlParameter("ActNbr", ActNbr)
        Dim parm2 As New SqlParameter("PriTranCode", PriTranCode)
        Dim pSecTranCode As New SqlParameter("SecTranCode", SecTranCode)
        Dim pStatus As New SqlParameter("Status", Status)
        Dim pCust As New SqlParameter("CustID", iCustID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(pSecTranCode)
        sqlCmd.Parameters.Add(pStatus)
        sqlCmd.Parameters.Add(pCust)
        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim ds As New DataSet
        da.Fill(ds)
        oConn.Close()
        Return ds
    End Function


    Public Function GetActiveAccounts(ByVal iCustID As Integer) As DataSet
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("select t1.actID,t1.buttontext,t2.accountTypeID," + _
        "t3.EntityID, t1.ableToDelete, t1.ActNbr " + _
        "from Accounts t1, Accounttypes t2, Entities t3 " + _
        "where(t1.ActTypeID = t2.AccountTypeID) " + _
        "and t1.Active = 1 and t1.DepositFlag = 1 " + _
        "and t3.EntityID = t1.EntityID " + _
        "and t1.CustomerID = " + iCustID.ToString, oConn.sqlConn)
        sqlCmd.CommandType = CommandType.Text

        Dim da As New SqlDataAdapter
        da.SelectCommand = sqlCmd
        Dim dsActs As New DataSet
        da.Fill(dsActs)

        Dim dt As New DataTable("AllAccounts")
        Dim ifxCol As New DataColumn
        Dim arrIFX As New ArrayList()
        For Each oRow In dsActs.Tables(0).Rows
            Dim oAcct As New clsAccount()
            For Each oCol In dsActs.Tables(0).Columns
                If oCol.Caption = "actID" Then
                    oAcct.ActID = oRow(oCol)
                ElseIf oCol.Caption = "buttontext" Then
                    oAcct.ButtonText = oRow(oCol)
                ElseIf oCol.Caption = "accountTypeID" Then
                    oAcct.ActTypeID = oRow(oCol)
                ElseIf oCol.Caption = "EntityID" Then
                    oAcct.EntityID = oRow(oCol)
                ElseIf oCol.Caption = "ableToDelete" Then
                    oAcct.AbleToDelete = oRow(oCol)
                ElseIf oCol.Caption = "date_time" Then
                    oAcct.CreateDate = oRow(oCol)
                ElseIf oCol.Caption = "ActNbr" Then
                    oAcct.ActNbr = oRow(oCol)
                End If
            Next
            arrIFX.Add(oAcct)
        Next
        dt.Columns.Add("ButtonText", Type.GetType("System.String"))
        dt.Columns.Add("Balance", Type.GetType("System.Double"))
        dt.Columns.Add("ActID", Type.GetType("System.String"))
        dt.Columns.Add("AccountTypeID", Type.GetType("System.String"))
        dt.Columns.Add("EntityID", Type.GetType("System.String"))
        dt.Columns.Add("AbleToDelete", Type.GetType("System.String"))

        For Each oAcct As clsAccount In arrIFX
            Dim dsEZ As DataSet = IFX_GetAccountInfo(oAcct.ActNbr)
            For Each oRow As DataRow In dsEZ.Tables(0).Rows
                For Each ezcol In dsEZ.Tables(0).Columns
                    If ezcol.Caption = "cashbalance" Then
                        oAcct.cashBalance = oRow(ezcol)
                    End If
                Next
            Next

            Dim editRow As DataRow = dt.NewRow()
            editRow(0) = oAcct.ButtonText
            editRow(1) = oAcct.cashBalance
            editRow(2) = oAcct.ActID
            editRow(3) = oAcct.ActTypeID
            editRow(4) = oAcct.EntityID
            editRow(5) = oAcct.AbleToDelete
            dt.Rows.Add(editRow)
        Next
        Dim dsRet As New DataSet
        dsRet.Tables.Add(dt)
        Return dsRet

    End Function

    Public Sub New()
        oConn = New clsConn()


    End Sub
    Public Function GetRandomNum(ByVal numDigits As Integer, ByVal max As Integer) As Integer
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("prcRandomNumber", oConn.sqlConn)
        Dim parm1 As New SqlParameter("numDigits", numDigits)
        Dim parm2 As New SqlParameter("numSides", max)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As Integer = 0
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader("iValue")
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Sub PrepareTestData()
        Dim parm1 As New SqlParameter("rand", GetRandomNum(3, 999))
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_PrepareTestData", oConn.sqlConn)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure

        sqlCmd.ExecuteNonQuery()
        oConn.Close()

    End Sub
    Public Function GetLangName(ByVal iLangCode As Integer) As String
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetLangName", oConn.sqlConn)
        Dim parm1 As New SqlParameter("LangID", iLangCode)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function
    Public Function GetLangID(ByVal strLangName As String) As Integer
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetLangCode", oConn.sqlConn)
        Dim parm1 As New SqlParameter("LangName", strLangName)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As Integer = 0
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            retVal = dataReader(0)
        End While
        dataReader.Close()
        oConn.Close()
        Return retVal
    End Function

    Public Function Decrypt(ByVal strEncrypted As String) As String
        Return TPSUtilities.AESEncryption.Decrypt(strEncrypted, "tr@ns@ct", "123-98pnw-f9pcj9-qruk1-2uh0q34yh", "SHA1", 2, "16CHARSLONG12345", 256)
    End Function

    Public Function Encrypt(ByVal strEncrypted As String) As String
        Return TPSUtilities.AESEncryption.Encrypt(strEncrypted, "tr@ns@ct", "123-98pnw-f9pcj9-qruk1-2uh0q34yh", "SHA1", 2, "16CHARSLONG12345", 256)
    End Function

    Public Function oDerivePasswordBytes(ByVal password As Byte(), salt As Byte(), ByVal hashName As String, ByVal iterations As Integer, ByVal keySize As Integer) As Byte()
        Return New PasswordDeriveBytes(password, salt, hashName, iterations).GetBytes(keySize / 8)
    End Function

    Public Function GetCompanyData(ByVal CompanyID As Integer) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetCompanies", oConn.sqlConn)
        Dim parm1 As New SqlParameter("CompanyNumber", CompanyID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        xmlDoc.AppendChild(root)
        While dataReader.Read
            Dim xmlEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "EntityID", "")
            xmlEle.InnerText = dataReader("EntityID")
            root.AppendChild(xmlEle)
            xmlEle = xmlDoc.CreateNode(XmlNodeType.Element, "Tier", "")
            If Not IsDBNull(dataReader("Tier")) Then
                xmlEle.InnerText = dataReader("Tier")
            Else
                xmlEle.InnerText = ""
            End If
            root.AppendChild(xmlEle)
            xmlEle = xmlDoc.CreateNode(XmlNodeType.Element, "BackgroundCheck", "")
            xmlEle.InnerText = dataReader("BackgroundCheck")
            root.AppendChild(xmlEle)
            xmlEle = xmlDoc.CreateNode(XmlNodeType.Element, "USPhotoID", "")
            xmlEle.InnerText = dataReader("USPhotoID")
            root.AppendChild(xmlEle)
            xmlEle = xmlDoc.CreateNode(XmlNodeType.Element, "USorForeignPhotoID", "")
            xmlEle.InnerText = dataReader("USorForeignPhotoID")
            root.AppendChild(xmlEle)
            xmlEle = xmlDoc.CreateNode(XmlNodeType.Element, "CustomerActivation", "")
            xmlEle.InnerText = dataReader("CustomerActivation")
            root.AppendChild(xmlEle)
        End While
        dataReader.Close()
        Return root

    End Function

    Public Function GetOldestCustomerCard(ByVal iCustID As Integer, ByVal PhotoID As String, ByVal DOB As Date, ByVal State As String, ByVal iWksID As Integer) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetOldestCustCard", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        Dim parm2 As New SqlParameter("PhotoID", PhotoID)
        Dim parm3 As New SqlParameter("DOB", DOB)
        Dim parm4 As New SqlParameter("State", State)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.Parameters.Add(parm3)
        sqlCmd.Parameters.Add(parm4)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")

        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            Dim pan As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CardID", "")
            pan.InnerText = dataReader("CardID")
            Dim CDType As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "CDType", "")
            CDType.InnerText = dataReader("CDType")
            root.AppendChild(pan)
            root.AppendChild(CDType)
        End While
        dataReader.Close()
        oConn.Close()
        Return root
    End Function

    Public Function GetMessageByType(ByVal msgType As String) As XmlElement
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_GetMessage", oConn.sqlConn)
        Dim parm1 As New SqlParameter("MsgType", msgType)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.CommandType = CommandType.StoredProcedure
        Dim retVal As String = ""
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        Dim dataReader As SqlDataReader = sqlCmd.ExecuteReader()
        While dataReader.Read
            Dim MsgHeader As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MsgHeader", "")
            MsgHeader.InnerText = dataReader("MsgHeader")
            Dim MsgBody As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MsgBody", "")
            MsgBody.InnerText = dataReader("MsgBody")
            Dim MsgFooter As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MsgFooter", "")
            MsgFooter.InnerText = dataReader("MsgFooter")
            Dim MsgTitleEle As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "MsgTitle", "")
            MsgTitleEle.InnerText = dataReader("MsgTitle")
            root.AppendChild(MsgHeader)
            root.AppendChild(MsgBody)
            root.AppendChild(MsgFooter)
            root.AppendChild(MsgTitleEle)
        End While
        dataReader.Close()
        oConn.Close()
        Return root
    End Function
    Public Function UpdateCardToHot(ByVal iCustID As Integer, ByVal cardID As Integer) As Boolean
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_UpdateCardToHot", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        Dim parm2 As New SqlParameter("iCardID", cardID)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm2)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True
    End Function
    Public Function SavePrintRecord(ByVal Message As String, ByVal PrintStatus As String, ByVal iCustID As Integer) As Boolean
        oConn.Connect(False)
        Dim sqlCmd As New SqlCommand("sp_SavePrintRecord", oConn.sqlConn)
        Dim parm1 As New SqlParameter("iCustID", iCustID)
        Dim parm5 As New SqlParameter("Message", Message)
        sqlCmd.Parameters.Add(parm1)
        sqlCmd.Parameters.Add(parm5)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.ExecuteNonQuery()
        oConn.Close()
        Return True
    End Function
    Public Sub UpdateCustomerAddress(ByVal strPhotoID As String, ByVal dDOB As Date, ByVal iCustID As Integer, ByVal Addr1 As String, ByVal Addr2 As String, _
                                          ByVal City As String, ByVal State As String, ByVal Zip As String, ByVal PhoneHome As String, ByVal PhoneWork As String)
        oConn.Connect(False)
        Dim oCmd As New SqlCommand("sp_UpdateAddress", oConn.sqlConn)
        oCmd.CommandType = CommandType.StoredProcedure
        Dim PhotoID As New SqlParameter("PhotoID", strPhotoID)
        oCmd.Parameters.Add(PhotoID)
        Dim DOB As New SqlParameter("DOB", dDOB)
        oCmd.Parameters.Add(DOB)
        Dim CustID As New SqlParameter("iCustID", iCustID)
        oCmd.Parameters.Add(CustID)
        Dim pAddr1 As New SqlParameter("Addr1", Addr1)
        oCmd.Parameters.Add(pAddr1)
        Dim pAddr2 As New SqlParameter("Addr2", Addr2)
        oCmd.Parameters.Add(pAddr2)
        Dim pCity As New SqlParameter("City", City)
        oCmd.Parameters.Add(pCity)
        Dim pState As New SqlParameter("State", State)
        oCmd.Parameters.Add(pState)

        Dim pZip As New SqlParameter("Zip", Zip)
        oCmd.Parameters.Add(pZip)
        Dim pPhoneHome As New SqlParameter("PhoneHome", PhoneHome)
        oCmd.Parameters.Add(pPhoneHome)
        Dim pPhoneWork As New SqlParameter("PhoneWork", PhoneWork)
        oCmd.Parameters.Add(pPhoneWork)
        oCmd.ExecuteNonQuery()

    End Sub

End Class
