Imports System.Xml
Imports System.Configuration
Public Class clsInterface
    Dim oChkService As New CheckService.ICheckServiceservice

    Public Function SelectNumberOfPendingReviews() As Integer
        Dim dt As DataTable = New clsData().SelectPendingReviews().Tables(0)
        For Each oRow In dt.Rows
            For Each oCol In dt.Columns
                Return Int32.Parse(oRow(oCol).ToString)
            Next
        Next
    End Function
    Public Function GetErrorData(ByVal iErrCode As Integer) As XmlElement
        Dim oData As New clsData
        Return oData.GetErrorData(iErrCode)
    End Function

    Public Function GetRegistrationDataFromPan(ByVal PAN As String) As XmlElement
        Dim oData As New clsData
        Return oData.GetRegistrationDataFromPAN(PAN)
    End Function

    Public Function GetCustomerDataFromPan(ByVal PAN As String) As XmlElement
        Dim oData As New clsData
        Return oData.GetCustomerDataFromPAN(PAN)
    End Function


    Public Function HelloWorld() As String
        Return "Hello World"
    End Function


    Public Function FillLanguages() As ArrayList
        Dim oData As New clsLanguage
        Return oData.FillLanguages
    End Function

    Public Function ValidateBin(ByVal iBin As Integer, ByVal iCustID As Integer) As String
        Dim oData As New clsData
        Return oData.ValidateBin(iBin, iCustID)
    End Function

    Public Function GetCurrentCaseNum()
        Dim oData As New clsData
        Return oData.GetCurrentCaseNum
    End Function

    Public Function GetTestData(ByVal strKey As String, ByVal CaseNum As Integer)
        Dim oData As New clsData
        Return oData.GetTestData(strKey, CaseNum)
    End Function

    Public Function InsRegReview(ByVal Scan_ID As Integer, ByVal GroupID As Integer, ByVal CustomerID As Integer, ByVal First_name As String, _
ByVal Last_Name As String, ByVal DOB As DateTime, ByVal Middle_Name As String, ByVal State_Code As String, ByVal ID_Type As Integer, _
ByVal Issue_date As DateTime, ByVal Expiration_date As DateTime, ByVal PHOTO_ID As String, ByVal Address1 As String, ByVal Address2 As String, _
ByVal City As String, ByVal Zip As String, ByVal Height As String, ByVal Weight As String, ByVal Eye_Color As String, ByVal Hair_Color As String, _
ByVal User_Name As String, ByVal Status As String, ByVal CustomerImageVerify As Boolean, ByVal PhotoIDMatches As Boolean, ByVal Createdate As DateTime, _
ByVal SSN As String, ByVal Sex As String, ByVal TransactionType As String, ByVal BK_Flag As Integer, ByVal PAN As String) As Integer
        Dim oData As New clsData
        Return oData.InsRegReview(Scan_ID, GroupID, CustomerID, First_name, _
 Last_Name, DOB, Middle_Name, State_Code, ID_Type, _
 Issue_date, Expiration_date, PHOTO_ID, Address1, Address2, _
 City, Zip, Height, Weight, Eye_Color, Hair_Color, _
 User_Name, Status, CustomerImageVerify, PhotoIDMatches, Createdate, _
 SSN, Sex, TransactionType, BK_Flag, PAN)
    End Function

    Public Sub UpdateRegTestData(ByVal Key As String)
        Dim oData As New clsData
        oData.UpdateRegTestData(Key)
    End Sub

    Public Function QueryCheckCodes(ByVal Key As String) As String
        Dim oData As New clsData
        Return oData.QuerycheckCodes(Key)
    End Function

    Public Sub UpdateRegReview(ByVal iScanID As Integer, ByRef strFirst As String, ByRef strlast As String, ByRef strDOB As String)
        Dim oData As New clsData
        oData.UpdateRegReview(iScanID, strFirst, strlast, strDOB)
    End Sub

    Public Sub UpdateCheckReview(ByVal iBlockID As Integer, ByVal mAmt As Double, ByVal dCheckDate As Date)
        Dim oData As New clsData
        oData.UpdateCheckReview(iBlockID, mAmt, dCheckDate)
    End Sub

    Public Function GetCheckCaseCode() As String
        Dim oData As New clsData
        Return oData.GetCheckCaseCode()
    End Function


    Public Sub UpdateCaseNum()
        Dim oData As New clsData
        oData.UpdateCaseNum()
    End Sub

    Public Sub DropRegTestData()
        Dim oData As New clsData
        oData.DropRegTestData()
    End Sub

    Public Sub InsChecks(ByVal iBlockID As Integer, ByVal iTransID As Integer, ByVal iCustomerID As Integer)
        Dim oData As New clsData
        oData.insChecks(iBlockID, iTransID, iCustomerID)
    End Sub

    Public Function SaveImage(ByVal strCode As String, ByVal arrImg As Byte(), ByVal iCustID As Integer, ByVal iBlockID As Integer, _
                         ByVal iWksId As Integer, ByRef iTranID As Integer, ByRef strPan As String, ByVal iActID As Integer, ByVal mAmt As Decimal) As XmlElement
        Dim oData As New clsData

        If iBlockID = 0 Then
            Dim xmlEle As XmlElement = oData.CreateTransactionNum(iCustID, strCode, "", iBlockID, iActID, mAmt)
            iTranID = CType(xmlEle.ChildNodes(0).InnerText, Integer)
            iBlockID = CType(xmlEle.ChildNodes(1).InnerText, Integer)
        End If
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "SaveImage", "")
        xmlDoc.AppendChild(root)
        Dim xmlItem As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "BlockID", "")
        xmlItem.InnerText = iBlockID.ToString
        root.AppendChild(xmlItem)
        xmlItem = xmlDoc.CreateNode(XmlNodeType.Element, "TranID", "")
        xmlItem.InnerText = iTranID.ToString
        root.AppendChild(xmlItem)
        oData.SaveImage(strCode, arrImg, iCustID, iBlockID, iWksId, strPan, iTranID)
        Return (xmlDoc.FirstChild)
    End Function


    Public Function GetImage(ByRef iBlockID As Integer, ByVal iCustId As Integer, ByVal strCode As String, ByVal strPan As String) As Byte()
        Dim oData As New clsData
        Return oData.GetImage(iCustId, iBlockID, strCode, strPan)

    End Function

    Public Function DerivePasswordBytes(ByVal password As Byte(), salt As Byte(), ByVal hashName As String, ByVal iterations As Integer, ByVal keySize As Integer) As Byte()
        Return New clsData().oDerivePasswordBytes(password, salt, hashName, iterations, keySize)
    End Function

    Public Function GetWorkstations() As ArrayList
        Return New clsData().GetWorkstations()
    End Function

    Public Function GetWorkstationID(ByVal IP As String) As Integer
        Return New clsData().GetWorkstationID(IP)
    End Function

    Public Sub PrepareTestData()
        Dim oData As New clsData
        oData.PrepareTestData()

    End Sub

    Public Function Encrypt(ByVal strEncrypt As String) As String
        Return New clsData().Encrypt(strEncrypt)
    End Function

    Public Function GetKioskSettings(ByVal strName As String, ByVal iWksID As Integer) As String
        Dim oData As New clsData
        Return oData.GetKioskSettings(strName, iWksID)
    End Function

    Public Function GetLangName(ByVal iLangId As Integer) As String
        Dim oData As New clsData()
        Return oData.GetLangName(iLangId)
    End Function

    Public Function GetTranslation(ByVal iLangId As Integer, ByVal strLabel As String) As String
        Dim oData As New clsData()
        Return oData.GetTranslation(strLabel, iLangId)
    End Function

    Public Function GetLangID(ByVal strLangName As String) As Integer
        Dim oData As New clsData()
        Return oData.GetLangID(strLangName)
    End Function


    Public Function GetCustomerDataFromScan(ByVal iScanID As Integer) As XmlNode
        Dim oData As New clsData
        Return oData.GetCustomerDataFromScan(iScanID)
    End Function

    Public Function GetCheckData(ByVal iCustID As Integer, ByVal iBlockID As Integer) As XmlElement
        Dim oData As New clsData
        Return oData.GetCheckData(iCustID, iBlockID)
    End Function


    Public Function GetCompaniesList() As List(Of clsCompany)
        Return New clsData().GetCompaniesList()
    End Function

    Public Function HasCheckBeenReviewed(ByVal strAcct As String, ByVal strRoute As String, ByVal strCheckNum As String, ByVal iCustID As Integer, ByVal iWksID As Integer) As XmlNode
        Dim oData As New clsData
        Return oData.HasCheckBeenReviewed(strAcct, strRoute, strCheckNum, iCustID, iWksID)
    End Function

    Public Function GetRandomNumber(ByVal numDigits As Integer, ByVal max As Integer) As Integer
        Dim oData As New clsData
        Return oData.GetRandomNum(numDigits, max)
    End Function

    Public Function GetCompanyData(ByVal CompanyID As Integer) As XmlElement
        Return New clsData().GetCompanyData(CompanyID)
    End Function

    Public Function GetActiveAccounts(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetActiveAccounts(iCustID)
    End Function

    Public Function GetHeldEZCashTrans(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetHeldEZCashTrans(iCustID)
    End Function

    Public Function GetAllTransactionsByBlock(ByVal iBlockID As Integer, ByVal iCustID As Integer, ByVal iTranID As Integer, ByVal acctDs As DataSet, ByVal feeCompDs As DataSet) As clsBlockDetails
        Return New clsData().GetAllTransactionsbyBlockAndCust(iBlockID, iCustID, iTranID, acctDs, feeCompDs)
    End Function

    Public Function GetEZCashAccounts(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetEZCashAccounts(iCustID)
    End Function

    Public Function GetFeeWebComps() As DataSet
        Return New clsData().IFX_GetFeeWebComp()
    End Function
    Public Function GetEZCashDepositAccounts(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetEZCashDepositAccounts(iCustID)
    End Function


    Public Function GetEZCashInactiveAccounts(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetEZCashInactiveAccounts(iCustID)
    End Function


    Public Function GetAllTransactionsDS(ByVal iCustID As Integer, ByVal acctDs As DataSet, ByVal dtFrom As String, ByVal dtTo As String) As List(Of clsTranRow)
        Return New clsData().GetAllTransactionsDS(iCustID, acctDs, dtFrom, dtTo)
    End Function

    Public Function GetAllTransactionsByAcct(ByVal iActID As Integer, ByVal dtFrom As String, ByVal dtTo As String) As List(Of clsTranRow)
        Return New clsData().GetAllTransactionsbyAccount(iActID, dtFrom, dtTo)
    End Function

    Public Function GetEZCashTransactions(ByVal iCustID As Integer) As DataSet
        Return New clsData().GetEZCashTransactions(iCustID)
    End Function

    Public Function IFX_GetTransactionByTranID(ByVal iTranID As Integer) As DataSet
        Return New clsData().IFX_GetTransactionsByTranID(iTranID)
    End Function


    Public Function GetEZCashTransactionsByBlockID(ByVal iBlockID As Integer, ByVal iCustID As Integer) As DataSet
        Return New clsData().GetEZCashTransactionsByBlockAndCust(iBlockID, iCustID)
    End Function

    Public Function GetIFXTransactionsByBlockIDAndCustID(ByVal iBlockID As Integer, ByVal iCustID As Integer) As DataSet
        Return New clsData().IFX_GetTransactionsByBlock(iBlockID, iCustID)
    End Function

    Public Function GetSystemSettings(ByVal strkey As String) As String
        Return New clsData().GetSystemSettings(strkey)
    End Function

    Public Function GetEZCashTransactionsByAcctID(ByVal ActID As String) As DataSet
        Return New clsData().GetEZCashTransactionsByAcct(ActID)
    End Function

    Public Function SaveCustomerCard(ByVal iCustID As Integer, ByVal strPan As String, ByVal strCDType As String, _
                                     ByVal bLoadable As Boolean, ByVal strActNbr As String, ByVal CompanyID As Integer) As Integer
        Dim oData As New clsData()
        Return oData.SaveCustomerCard(iCustID, strPan, strCDType, bLoadable, strActNbr, CompanyID)
    End Function

    Public Function GetUserSalt(ByVal CustomerID) As String

        Return New clsData().GetUserSalt(CustomerID)
    End Function

    Public Function GetCustomerDataFromID(ByVal strPhotoID As String, ByVal dDOB As DateTime, ByVal strState As String,
                                          ByVal iCustomerID As Integer, ByVal strRouteNumber As String, _
                                          ByVal strAccountNumber As String, ByVal iCheckNumber As Integer, _
                                          ByVal mNetAmt As Decimal, ByVal iWksID As Integer) As XmlNode
        Dim oData As New clsData
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        Dim ReturnCode1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode1", "")
        ReturnCode1.InnerText = "0"
        Dim ReturnCode2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode2", "")
        Dim ReturnCode3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode3", "")
        Try

            Dim xmlParts As XmlElement = oData.GetCustomerDataFromID(strPhotoID, dDOB, strState, iCustomerID, strRouteNumber, strAccountNumber, iCheckNumber, mNetAmt, iWksID)
            If xmlParts.ChildNodes.Count = 0 Then
                ReturnCode3.InnerText = "X"
                root.AppendChild(ReturnCode1)
                root.AppendChild(ReturnCode3)
                Return root
            End If


            Return xmlParts
        Catch ex As Exception
            ReturnCode1.InnerText = "1'"
            root.AppendChild(ReturnCode1)
            Return root
        End Try

    End Function

    Public Function PostLostCard(ByVal strPhotoID As String, ByVal dDOB As DateTime, ByVal strState As String,
                                          ByVal iCustomerID As Integer, ByVal LostCardCode As Integer, ByVal strNewPan As String, _
                                          iWksID As Integer, ByVal strProviderID As String) As XmlNode
        Dim oData As New clsData
        Dim xmlDoc As New XmlDocument
        Dim iOnFileLostCardCode As Integer = 0
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        Dim ReturnCode1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode1", "")
        ReturnCode1.InnerText = "0"
        Dim ReturnCode3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode3", "")
        Dim ReturnCode2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode2", "")
        ReturnCode2.InnerText = "0"
        Try

            Dim xmlParts As XmlElement = oData.GetCustomerDataFromID(strPhotoID, dDOB, strState, iCustomerID, "", "", 0, 0, iWksID)
            If xmlParts.ChildNodes.Count = 0 Then
                ReturnCode2.InnerText = "X"
                root.AppendChild(ReturnCode1)
                root.AppendChild(ReturnCode2)
                Return root
            End If
            For Each xmlPart As XmlNode In xmlParts.ChildNodes
                If xmlPart.Name = "LostCardCode" Then
                    iOnFileLostCardCode = Int32.Parse(xmlPart.InnerText)
                End If
            Next
            If (LostCardCode = 0) Or (iOnFileLostCardCode <> LostCardCode) Then
                ReturnCode2.InnerText = "I"
                root.AppendChild(ReturnCode1)
                root.AppendChild(ReturnCode2)
                Return root
            End If
            While True
                Dim xmlEle As XmlNode = New clsData().GetOldestCustomerCard(iCustomerID, strPhotoID, dDOB, strState, iWksID)
                If xmlEle.ChildNodes.Count = 0 Then
                    ReturnCode2.InnerText = "N"
                    root.AppendChild(ReturnCode1)
                    root.AppendChild(ReturnCode2)
                    Return root
                End If
                Dim oQueue As New clsQueue(iWksID)
                oQueue.CustomerID = iCustomerID
                oQueue.QueueCode = "IC"
                oQueue.ProviderID = strProviderID
                oQueue.Data2 = strNewPan
                oQueue.Data3 = xmlEle.ChildNodes(1).InnerText
                oQueue.Data4 = "2"
                oQueue.Data5 = xmlEle.FirstChild().InnerText
                oQueue.PostToQueue()
                If IsDBNull(oQueue.ReturnCode2) Then
                    ReturnCode1.InnerText = "1"
                    root.AppendChild(ReturnCode1)
                    Return root
                End If
                If Int32.Parse(oQueue.ReturnCode2) > 0 Then
                    ReturnCode2.InnerText = "E"
                    root.AppendChild(ReturnCode1)
                    root.AppendChild(ReturnCode2)
                    Return root
                Else
                    ReturnCode2.InnerText = "D"
                    root.AppendChild(ReturnCode1)
                    root.AppendChild(ReturnCode2)
                End If
            End While
            Dim seg1 As New clsLabel
            seg1.strLabel = "SDReg"
            seg1.iTranslate = 1
            Dim seg2 As New clsLabel
            seg2.strLabel = "PasswordEntry"
            seg2.iTranslate = 1
            Dim seg3 As New clsLabel
            seg3.strLabel = oData.GetRandomNum(3, 999).ToString().PadRight(3, "0") + oData.GetRandomNum(3, 999).ToString().PadRight(3, "0") + oData.GetRandomNum(2, 99).ToString().PadRight(2, "0")
            seg3.iTranslate = 0
            Dim arrSeg(3) As clsLabel
            arrSeg(0) = seg1
            arrSeg(1) = seg2
            arrSeg(2) = seg3
            COMMS(iCustomerID, arrSeg)

        Catch ex As Exception
            ReturnCode1.InnerText = ex.ToString
            root.AppendChild(ReturnCode1)
            Return root
        End Try

    End Function

    Public Function Test() As XmlNode
        Dim oData As New clsData
        Dim seg1 As New clsLabel
        seg1.strLabel = "CMReg1"
        seg1.iTranslate = 1
        Dim seg2 As New clsLabel
        seg2.strLabel = "PasswordEntry"
        seg2.iTranslate = 1
        Dim seg3 As New clsLabel
        seg3.strLabel = oData.GetRandomNum(3, 999).ToString().PadRight(3, "0") + oData.GetRandomNum(3, 999).ToString().PadRight(3, "0") + oData.GetRandomNum(2, 99).ToString().PadRight(2, "0")
        seg3.iTranslate = 0
        Dim arrSeg(2) As clsLabel
        arrSeg(0) = seg1
        arrSeg(1) = seg2
        arrSeg(2) = seg3
        Return COMMS(11297, arrSeg)
    End Function

    Public Function GetCustomerPhotoID(ByVal Barcode As Integer) As DataSet
        Return New clsData().GetCustomerPhotoID(Barcode)
    End Function

    Public Function SetCustomerPhotoID(ByVal PhotoIDsID As Integer, ByVal CustomerID As Integer)
        Return New clsData().SetCustomerPhotoID(PhotoIDsID, CustomerID)
    End Function

    Public Function GetCheckCodeText(ByVal strCode As String) As String
        Dim oData As New clsData
        Return oData.GetCheckCodeText(strCode)
    End Function

    Public Function GetRandomNum(ByVal numDigits As Integer, ByVal max As Integer) As Integer
        Dim oData As New clsData
        Return oData.GetRandomNum(numDigits, max)
    End Function

    Public Sub UpdateCustomer(ByVal Password As String, ByVal iCustID As Integer)
        Dim oData As New clsData
        oData.UpdateCustomer(Password, iCustID)
    End Sub

    Public Function CreateTransactionNum(ByVal iCustId As Integer, ByVal tran_type As String, ByVal sec_tran_type As String, _
                                         ByVal iBlockID As Integer, ByVal iActID As Integer, ByVal mAmt As Decimal) As XmlNode
        Dim oData As New clsData
        Return oData.CreateTransactionNum(iCustId, tran_type, sec_tran_type, iBlockID, iActID, mAmt)
    End Function

    Public Function SaveEntityAccountType(ByVal oEntActType As clsEntityAccountType) As Boolean
        Return New clsData().SaveEntityActType(oEntActType)
    End Function

    Public Function GetEntityAccountType(ByRef oEntActType As clsEntityAccountType) As clsEntityAccountType
        Return New clsData().GetEntityActType(oEntActType)
    End Function

    Public Function SaveCompany(ByVal oCompany As clsCompany) As Boolean
        Return New clsData().SaveCompany(oCompany)
    End Function

    Public Function GetCompany(ByRef oCompany As clsCompany) As clsCompany
        Return New clsData().GetCompany(oCompany)
    End Function

    Public Function GetCompanies() As DataSet
        Return New clsData().GetAllCompanies()
    End Function

    Public Function GetInactiveCompanies() As DataSet
        Return New clsData().GetInactiveCompanies()
    End Function

    Public Function GetEntities() As DataSet
        Return New clsData().GetAllEntities()
    End Function

    Public Function GetCompaniesByEntity(ByVal EntityID As Integer) As DataSet
        Return New clsData().GetCompaniesByEntity(EntityID)
    End Function

    Public Function GetInActiveCompaniesByEntity(ByVal EntityID As Integer) As DataSet
        Return New clsData().GetInactiveCompaniesByEntityID(EntityID)
    End Function

    Public Function GetActInfo(ByVal ActID As Integer) As DataSet
        Dim oData As New clsData()
        Return oData.IFX_GetAccountInfo(oData.GetActNbr(ActID))
    End Function

    Public Function MarkAllCardsHot(ByVal NewPan As String, iWKSID As Integer, iCustID As Integer, PhotoID As String, DOB As Date, State As String, ProviderID As Integer) As String
        Dim oData As New clsData
        Dim strRet As String = "0"
        While strRet = "0"
            strRet = MarkCardHot(NewPan, iWKSID, iCustID, PhotoID, DOB, State, ProviderID)
        End While
        Return strRet
    End Function



    Public Function MarkCardHot(ByVal NewPan As String, iWKSID As Integer, iCustID As Integer, PhotoID As String, DOB As Date, State As String, ProviderID As Integer) As String
        Dim oData As New clsData
        Dim xmlEle As XmlElement = oData.GetCustomerDataFromID(PhotoID, DOB, State, iCustID, "", "", 0, 0, iWKSID)
        If xmlEle.ChildNodes.Count = 0 Then
            Return "X"
        End If


        xmlEle = New clsData().GetOldestCustomerCard(iCustID, PhotoID, DOB, State, iWKSID)
        If xmlEle.ChildNodes.Count = 0 Then
            Return "N"
        End If
        Dim oQueue As New clsQueue(iWKSID)
        oQueue.CustomerID = iCustID
        oQueue.QueueCode = "IC"
        oQueue.ProviderID = ProviderID
        oQueue.Data2 = NewPan
        oQueue.Data3 = xmlEle.ChildNodes(1).InnerText
        oQueue.Data4 = "2"
        oQueue.Data5 = xmlEle.FirstChild().InnerText
        oQueue.PostToQueue()


        oData.UpdateCardToHot(iCustID, xmlEle.FirstChild().InnerText)
        Return "0"
    End Function
    Public Function COMMS(ByVal iCustID As Integer, ByVal strSeg As clsLabel()) As XmlElement
        oChkService.Url = ConfigurationManager.AppSettings("CheckServiceURL")
        oChkService.Trace("COMMS started for CustID=" + iCustID.ToString())
        Dim xmlDoc As New XmlDocument
        Dim oData As New clsData()
        Dim bSMS As Boolean = False
        Dim bEmail As Boolean = False
        Dim bLetter As Boolean = False
        Dim strPhone As String = ""
        Dim strEmail As String = ""
        Dim strSubject As String = ""
        Dim iLangID As Integer = 0
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        Dim ReturnCode1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode1", "")
        ReturnCode1.InnerText = "0"
        Dim ReturnCode2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode2", "")
        Dim ReturnCode3 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode3", "")
        Dim ReturnCode4 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode4", "")
        Try

            Dim xmlParts As XmlElement = oData.GetCustomerDataFromIDForCOMMS("", Date.Now, "", iCustID, "", 0)


            If xmlParts.ChildNodes.Count = 0 Then
                ReturnCode2.InnerText = "X"
                root.AppendChild(ReturnCode1)
                root.AppendChild(ReturnCode2)
                oChkService.Trace("Customer not found")
                Return root
            End If
            oChkService.Trace("Customer Count:" + xmlParts.ChildNodes.Count.ToString())
            For Each xmlPart As XmlNode In xmlParts
                If xmlPart.Name = "MessageSMS" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        bSMS = False
                    Else
                        bSMS = Int32.Parse(xmlPart.InnerText)
                    End If
                ElseIf xmlPart.Name = "MessageEmail" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        bEmail = False
                    Else
                        bEmail = Int32.Parse(xmlPart.InnerText)
                    End If
                ElseIf xmlPart.Name = "MessageLetter" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        bLetter = False
                    Else
                        bLetter = Int32.Parse(xmlPart.InnerText)
                    End If
                ElseIf xmlPart.Name = "PhoneMobile" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        strPhone = ""
                    Else
                        strPhone = xmlPart.InnerText
                    End If
                ElseIf xmlPart.Name = "Email" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        strEmail = ""
                    Else
                        strEmail = xmlPart.InnerText
                    End If
                ElseIf xmlPart.Name = "iLangID" Then
                    If IsDBNull(xmlPart.InnerText) Or xmlPart.InnerText = "" Then
                        iLangID = 1
                    Else
                        iLangID = xmlPart.InnerText
                    End If
                End If

            Next
            Dim strMsg As String = BuildCustomerMessage(strSeg, iLangID)
            oChkService.Trace("Customer Message:" + strMsg)
            If strMsg = "" Then
                ReturnCode2.InnerText = "B"
                root.AppendChild(ReturnCode1)
                root.AppendChild(ReturnCode2)
                Return root
            End If
            If (bSMS And strPhone = "") Or (bSMS And strPhone.Length <> 10) Then
                ReturnCode2.InnerText = "M"
                bLetter = False

            ElseIf bSMS Then
                oData.IFX_InsertMessage(strEmail, strPhone, 1, "", strMsg, 0)
            End If

            If (bEmail And strEmail = "") Or (bEmail And strEmail.Contains("@") = False) Then
                ReturnCode2.InnerText = "E"
            ElseIf bEmail Then
                oData.IFX_InsertMessage(strEmail, strPhone, 2, strSubject, strMsg, 1)
            End If

            If (bLetter) Then

                oData.SavePrintRecord(strMsg, 0, iCustID)
            End If

            ReturnCode4.InnerText = "S"
            root.AppendChild(ReturnCode1)
            root.AppendChild(ReturnCode2)
            root.AppendChild(ReturnCode3)
            root.AppendChild(ReturnCode4)
            Return root
        Catch ex As Exception
            ReturnCode1.InnerText = ex.ToString
            oChkService.Trace("Exception: " + ex.ToString())
            root.AppendChild(ReturnCode1)
            Return root
        End Try

    End Function

    Public Function UpdateCustomerAddress(ByVal strPhotoID As String, ByVal dDOB As Date, ByVal iCustID As Integer, ByVal Addr1 As String, ByVal Addr2 As String, _
                                          ByVal City As String, ByVal State As String, ByVal Zip As String, ByVal PhoneHome As String, ByVal PhoneWork As String, ByVal iWksID As Integer) As XmlElement
        Dim xmlDoc As New XmlDocument
        Dim root As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "root", "")
        Dim ReturnCode1 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode1", "")
        ReturnCode1.InnerText = "0"
        Dim ReturnCode2 As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ReturnCode2", "")
        Try
            Dim oData As New clsData
            Dim xmlParts = oData.GetCustomerDataFromID("", dDOB, "", iCustID, "", "", 0, 0, iWksID)
            If xmlParts.ChildNodes.Count = 0 Then
                root.AppendChild(ReturnCode1)
                ReturnCode2.InnerText = "X"
                root.AppendChild(ReturnCode2)
                Return root
            End If
            oData.UpdateCustomerAddress(strPhotoID, dDOB, iCustID, Addr1, Addr2, City, State, Zip, PhoneHome, PhoneWork)
            root.AppendChild(ReturnCode1)
            root.AppendChild(ReturnCode2)
            Return root
        Catch ex As Exception
            ReturnCode1.InnerText = "1"
            root.AppendChild(ReturnCode1)
            Return root
        End Try
    End Function


    Public Function BuildCustomerMessage(ByVal strSegs As clsLabel(), ByVal iLangCode As String) As String
        Dim strRet As String = ""
        Dim oData As New clsData()
        For Each strSeg As clsLabel In strSegs
            If strSeg.iTranslate = 1 Then
                strRet += oData.GetTranslation(strSeg.strLabel, iLangCode) + " "
            Else
                strRet += strSeg.strLabel + " "
            End If
        Next
        Return strRet
    End Function

    Public Function decrypt(ByVal strEncrypted As String) As String
        Return New clsData().Decrypt(strEncrypted)
    End Function

    'Removed by Roshelle 2/13/14
    'Public Function CreatePasswordHash(ByVal strPass As String, ByVal strSalt As String) As String
    '    Return Transact.Data.Core.Common.SaltLogic.CreatePasswordHash(strPass.Trim(), strSalt)

    'End Function

    Public Function RetrieveCompanyNumberForPan(ByVal Bin As String, ByVal CardHolder As String) As Integer
        Return New clsData().IFX_RetrieveCompanyNumberForPan(Bin, CardHolder)
    End Function

    Public Function RetrieveBarcodeForPan(ByVal Bin As String, ByVal CardHolder As String) As Integer
        Return New clsData().IFX_RetrieveBarcodeForPan(Bin, CardHolder)
    End Function

    Public Function MatchPreregData(ByVal Barcode As Integer) As DataSet
        Dim oData As New clsData()
        Return oData.MatchPreregData(Barcode)
    End Function

    Public Sub New()

    End Sub
End Class
