﻿Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Public Class Form1

#Region "Structures"
    Public Structure DaysOfWeek
        Dim Monday As String
        Dim Tuesday As String
        Dim Wednesday As String
        Dim Thursday As String
        Dim Friday As String
    End Structure
    Public Structure CalendarEvent
        Dim className As String
        Dim classDays As String
        Dim classTime As String
        Dim location As String
    End Structure
#End Region

#Region "Global Vars"
    Dim bclicked As Boolean = False
    Dim submitTLink As HtmlElement
#End Region

#Region "Helper Functions"
    Function findDays(daything As String)
        Dim days As String = ""

        If daything.Contains("Monday") Then
            days = days & "MO "
        End If
        If daything.Contains("Tuesday") Then
            days = days & "TU "
        End If
        If daything.Contains("Wednesday") Then
            days = days & "WE "
        End If
        If daything.Contains("Thursday") Then
            days = days & "TH "
        End If
        If daything.Contains("Friday") Then
            days = days & "FR "
        End If
        If daything.Contains("Saturday") Then
            days = days & "SA "
        End If

        Return days
    End Function
    Function CreateCalendar()
        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VCALENDAR")
        sb.AppendLine("PRODID:-//Google Inc//Google Calendar 70.9054//EN")
        sb.AppendLine("VERSION:2.0")
        sb.AppendLine("CALSCALE:GREGORIAN")
        sb.AppendLine("METHOD:PUBLISH")
        sb.AppendLine("X-WR-CALNAME:Test Calendar")
        sb.AppendLine("X-WR-TIMEZONE:America/Los_Angeles")
        sb.AppendLine("X-WR-CALDESC:This should be the description")
        sb.AppendLine("BEGIN:VTIMEZONE")
        sb.AppendLine("TZID:America/Los_Angeles")
        sb.AppendLine("X-LIC-LOCATION:America/Los_Angeles")
        sb.AppendLine("BEGIN:DAYLIGHT")
        sb.AppendLine("TZOFFSETFROM:-0800")
        sb.AppendLine("TZOFFSETTO:-0700")
        sb.AppendLine("TZNAME:PDT")
        sb.AppendLine("DTSTART:19700308T020000")
        sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU")
        sb.AppendLine("END:DAYLIGHT")
        sb.AppendLine("BEGIN:STANDARD")
        sb.AppendLine("TZOFFSETFROM:-0700")
        sb.AppendLine("TZOFFSETTO:-0800")
        sb.AppendLine("TZNAME:PST")
        sb.AppendLine("DTSTART:19701101T020000")
        sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU")
        sb.AppendLine("END:STANDARD")
        sb.AppendLine("END:VTIMEZONE")
        TextBox1.AppendText(sb.ToString)
        Return True
    End Function
    Function FinalizeCalendar()
        TextBox1.AppendText("END:VCALENDAR")
        File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics").Dispose()
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics", TextBox1.Text, False)
        MsgBox("Done! There is now a file on your desktop named my_schedule.ics. Please take that file and upload it to your google calendar.")
        Return True
    End Function
#End Region
    Sub find_classes(source As String)
        If (CreateCalendar()) Then
            Dim delim As String() = New String(0) {"<h4>"}
            Dim splitByDays = source.Split(delim, StringSplitOptions.None)
            'Dim splitByDays = source.Split("<h4>")
            For i = 1 To splitByDays.Length - 1
                Dim day As New Regex("([a-zA-Z]*)<\/h4>")
                Dim classes As New Regex("\s*.*\s*.*\s*<label>\s*([a-zA-Z0-9:\s-]*)<br\s\/>\s*.*\s*.*\s*([a-zA-Z0-9-\s]* )\s*[a-zA-Z<\s="":/\.-]*>([a-zA-Z0-9\s]*).*\s*.*\s*.*\s*")

                Dim dayMatches As Match = Regex.Match(splitByDays(i), day.ToString)
                Dim classMatches As MatchCollection = Regex.Matches(splitByDays(i), classes.ToString)

                For Each foundClass As Match In classMatches
                    Dim className As String = foundClass.Groups(2).Value
                    Dim classTime As String = foundClass.Groups(1).Value
                    Dim classDays As String = findDays(dayMatches.Groups(1).Value)
                    Dim classLocation As String = foundClass.Groups(3).Value
                    Dim my_evt As New CalendarEvent
                    my_evt.location = classLocation
                    my_evt.classTime = classTime
                    my_evt.className = className
                    my_evt.classDays = classDays
                    TextBox2.AppendText("Found Class: " & className.Replace("	", "") & "from " & classTime & " on " & dayMatches.Groups(1).Value & vbNewLine)
                    append_events(my_evt)

                Next
            Next
        End If
    End Sub

    Sub append_events(evt As CalendarEvent)


        Dim startTime As String = evt.classTime.Split("-")(0)

        '' If 
        If startTime.Contains("pm") Then
            startTime.Replace("p", "").Replace("m", "").Replace(" ", "")


            If Not startTime.Split(":")(0) = 12 Then
                startTime = (startTime.Split(":")(0) + 12) Mod 24
            End If
        ElseIf startTime.Contains("a") Then
            startTime.Replace("a", "").Replace("m", "").Replace(" ", "")
            startTime = startTime.Split(":")(0)
            If Convert.ToInt32(startTime) < 10 Then startTime = "0" & startTime
        End If


        Dim endTime As String = evt.classTime.Split("-")(1)

        If endTime.Contains("p") Then
            endTime.Replace("p", "").Replace("m", "").Replace(" ", "")
            If Not endTime.Split(":")(0) = 12 Then
                endTime = (endTime.Split(":")(0) + 12) Mod 24
            Else
                endTime = endTime.Split(":")(0)
            End If
        ElseIf endTime.Contains("a") Then
            endTime.Replace("a", "").Replace("m", "").Replace(" ", "")
            endTime = endTime.Split(":")(0)
            If Convert.ToInt32(endTime) < 10 Then endTime = "0" & endTime
        End If


        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VEVENT")
        Dim weekday As String
        Select Case evt.classDays.Replace(" ", "")
            Case "MO"
                weekday = "0330"
            Case "TU"
                weekday = "0331"
            Case "WE"
                weekday = "0401"
            Case "TH"
                weekday = "0402"
            Case "FR"
                weekday = "0403"
        End Select
        sb.AppendLine("DTSTART;TZID=America/Los_Angeles:2015" & weekday & "T" & (startTime).Split(":")(0) & evt.classTime.Split("-")(0).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        sb.AppendLine("DTEND;TZID=America/Los_Angeles:2015" & weekday & "T" & (endTime) & evt.classTime.Split("-")(1).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=20150605T140000Z;BYDAY=" & evt.classDays.Replace(" ", ""))
        sb.AppendLine("DTSTAMP:20150105T060809Z")
        sb.AppendLine("UID:" & System.Guid.NewGuid.ToString() & "@google.com")
        sb.AppendLine("CREATED:20150105T060759Z")
        sb.AppendLine("DESCRIPTION:")
        sb.AppendLine("LAST-MODIFIED:20150117T060759Z")
        If evt.location.Contains("Wait List") Then
            evt.className = evt.location
            evt.location = ""
        End If
        sb.AppendLine("LOCATION:" & evt.location)
        sb.AppendLine("SEQUENCE:0")
        sb.AppendLine("STATUS:CONFIRMED")
        sb.AppendLine("SUMMARY:" & evt.className)
        sb.AppendLine("TRANSP:OPAQUE")
        sb.AppendLine("END:VEVENT")
        TextBox1.AppendText(sb.ToString)

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button3.Click
        WebBrowser1.Navigate("https://act.ucsd.edu/myTritonlink20/mobile.htm")
        bclicked = True
        Button3.Enabled = False
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        submitTLink.Focus() 'Give the check button the focus
        SendKeys.Send("{ENTER}") 'Causes the validation of the check button
        Timer1.Stop() 'Stops the timer 
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GroupBox2.Enabled = False
        GroupBox3.Enabled = False
        TextBox4.UseSystemPasswordChar = True
    End Sub
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        GroupBox2.Enabled = True
        GroupBox3.Enabled = True
        If WebBrowser1.Url.ToString = "https://a4.ucsd.edu/tritON/Authn/UserPassword" And bclicked = True Then
            submitTLink = WebBrowser1.Document.GetElementById("ssopassword")
            WebBrowser1.Document.All("ssousername").InnerText = TextBox3.Text
            WebBrowser1.Document.All("ssopassword").InnerText = TextBox4.Text
            TextBox2.AppendText("Logging in..." & vbNewLine)
            System.Threading.Thread.Sleep(1000)
            Timer1.Start()
        End If

        If WebBrowser1.DocumentText.Contains("View Finals") Then
            TextBox2.AppendText("Logged in to Tritonlink" & vbNewLine)
            find_classes(WebBrowser1.DocumentText)
            WebBrowser2.Navigate("https://act.ucsd.edu/myTritonlink20/finalsmobile.htm?termcode=SP15")

        End If
    End Sub

#Region "Finals"
    Private Sub WebBrowser2_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser2.DocumentCompleted
        If WebBrowser2.DocumentText.Contains("Log Out") Then
            find_finals(WebBrowser2.DocumentText)
        End If

    End Sub
    Sub find_finals(source As String)
        Dim delim As String() = New String(0) {"<h4>"}
        Dim splitByDays = source.Split(delim, StringSplitOptions.None)
        'Dim splitByDays = source.Split("<h4>")
        For i = 1 To splitByDays.Length - 1
            Dim day As New Regex("([a-zA-Z]*)<\/h4>")
            Dim classes As New Regex("\s*.*\s*.*\s*<label>\s*([a-zA-Z0-9:\s-]*)<br\s\/>\s*.*\s*.*\s*([a-zA-Z0-9-\s]* )\s*[a-zA-Z<\s="":/\.-]*>([a-zA-Z0-9\s-]*).*\s*.*\s*.*\s*")

            Dim dayMatches As Match = Regex.Match(splitByDays(i), day.ToString)
            Dim classMatches As MatchCollection = Regex.Matches(splitByDays(i), classes.ToString)

            For Each foundClass As Match In classMatches
                Dim className As String = foundClass.Groups(2).Value
                Dim classTime As String = foundClass.Groups(1).Value
                Dim classDays As String = findDays(dayMatches.Groups(1).Value)
                Dim classLocation As String = foundClass.Groups(3).Value
                Dim my_evt As New CalendarEvent
                my_evt.location = classLocation
                my_evt.classTime = classTime
                my_evt.className = className
                my_evt.classDays = classDays
                TextBox2.AppendText("Found Final: " & className.Replace("	", "") & "from " & classTime & " on " & dayMatches.Groups(1).Value & vbNewLine)
                append_final_events(my_evt)

            Next
        Next

        TextBox1.AppendText("END:VCALENDAR")
        File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics").Dispose()
        '  System.Diagnostics.Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2")
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics", TextBox1.Text, False)

        MsgBox("Done! There is now a file on your desktop named my_schedule.ics. Please take that file and upload it to your google calendar.")
    End Sub
    Sub append_final_events(evt As CalendarEvent)


        Dim startTime As String = evt.classTime.Split("-")(0)

        '' If 
        If startTime.Contains("pm") Then
            startTime.Replace("p", "").Replace("m", "").Replace(" ", "")


            If Not startTime.Split(":")(0) = 12 Then
                startTime = (startTime.Split(":")(0) + 12) Mod 24
            End If
        ElseIf startTime.Contains("a") Then
            startTime.Replace("a", "").Replace("m", "").Replace(" ", "")
            startTime = startTime.Split(":")(0)
            If Convert.ToInt32(startTime) < 10 Then startTime = "0" & startTime
        End If


        Dim endTime As String = evt.classTime.Split("-")(1)

        If endTime.Contains("p") Then
            endTime.Replace("p", "").Replace("m", "").Replace(" ", "")
            If Not endTime.Split(":")(0) = 12 Then
                endTime = (endTime.Split(":")(0) + 12) Mod 24
            Else
                endTime = endTime.Split(":")(0)
            End If
        ElseIf endTime.Contains("a") Then
            endTime.Replace("a", "").Replace("m", "").Replace(" ", "")
            endTime = endTime.Split(":")(0)
            If Convert.ToInt32(endTime) < 10 Then endTime = "0" & endTime
        End If


        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VEVENT")
        Dim weekday As String
        Select Case evt.classDays.Replace(" ", "")
            Case "MO"
                weekday = "0608"
            Case "TU"
                weekday = "0609"
            Case "WE"
                weekday = "0610"
            Case "TH"
                weekday = "0611"
            Case "FR"
                weekday = "0612"
            Case "SA"
                weekday = "0606"
        End Select
        sb.AppendLine("DTSTART;TZID=America/Los_Angeles:2015" & weekday & "T" & (startTime).Split(":")(0) & evt.classTime.Split("-")(0).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        sb.AppendLine("DTEND;TZID=America/Los_Angeles:2015" & weekday & "T" & (endTime) & evt.classTime.Split("-")(1).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        'sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=20150614T140000Z;BYDAY=" & evt.classDays.Replace(" ", ""))
        sb.AppendLine("DTSTAMP:20150105T060809Z")
        sb.AppendLine("UID:" & System.Guid.NewGuid.ToString() & "@google.com")
        sb.AppendLine("CREATED:20150105T060759Z")
        sb.AppendLine("DESCRIPTION:")
        sb.AppendLine("LAST-MODIFIED:20150117T060759Z")
        sb.AppendLine("LOCATION:" & evt.location)
        sb.AppendLine("SEQUENCE:0")
        sb.AppendLine("STATUS:CONFIRMED")
        sb.AppendLine("SUMMARY:" & evt.className + " Final")
        sb.AppendLine("TRANSP:OPAQUE")
        sb.AppendLine("END:VEVENT")
        TextBox1.AppendText(sb.ToString)

    End Sub
#End Region

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        Dim x As New ClassesToAdd
        x.Show()
    End Sub
End Class

'#Region "Google"
'    Dim submitGoogle As HtmlElement
'    Function googleLogin()
'        submitGoogle = WebBrowser2.Document.GetElementById("Passwd")
'        WebBrowser2.Document.All("Email").InnerText = "nick.crow2@gmail.com"
'        WebBrowser2.Document.All("Passwd").InnerText = "scarbird"
'        Timer2.Start()
'        Return True
'    End Function
'    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
'        submitGoogle.Focus()
'        SendKeys.SendWait("{ENTER}")
'        Timer2.Stop()
'    End Sub
'    'https://accounts.google.com/ServiceLogin?service=cl&passive=1209600&continue=https://www.google.com/calendar/render&followup=https://www.google.com/calendar/render&scc=1
'    Dim c As New CookieContainer
'    Sub CheckCalendar()
'        Dim uri As New System.Uri("https://accounts.google.com/ServiceLogin?service=cl&passive=1209600&continue=https://www.google.com/calendar/render&followup=https://www.google.com/calendar/render&scc=1")
'        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(uri)
'        request.CookieContainer = c
'        request.CookieContainer.SetCookies(uri, WebBrowser2.Document.Cookie)
'        Dim response1 As System.Net.HttpWebResponse = request.GetResponse
'        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response1.GetResponseStream())
'        Dim response As String = sr.ReadToEnd
'        TextBox2.Text = response.ToString
'        WebBrowser2.DocumentText = response
'    End Sub

'#End Region

'#Region "GOOGLEAPI"


'    Sub test()

'        Dim credential As UserCredential
'        '  scopes.Add(CalendarService.Scope.Calendar)


'        Using stream As New FileStream("client_secrets_5.json", FileMode.Open, FileAccess.Read)
'            '     credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, scopes, "user", CancellationToken.None, New FileDataStore("CalendarProgram")).Result
'        End Using

'        Dim calendar As New Calendar()

'        calendar.Summary = "Calendar Sample"
'        calendar.TimeZone = "Asia/Manila"
'        calendar.Location = "Philippines"
'        calendar.Description = "calendar sample description"

'        '  Dim createdCalendar As Calendar = .myCalendarservice.Calendars.Insert(calendar).Execute()

'    End Sub
'#End Region
'#Region "GoogleStuff"


'    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
'        Dim x As New ClsGoogle()
'        x.GetCalendar(New Date(2000, 1, 1), Today)

'    End Sub
'#End Region
'Public Class ClsGoogle


'    '' Calendar scopes which is initialized on the main method.
'    Dim scopes As IList(Of String) = New List(Of String)()

'    '' Calendar service.
'    Dim service As CalendarService

'    Public CalEvents As List(Of [Event]) = New List(Of [Event])     'List of events in the calendar

'    Sub New()       'Classes constructor, to authenticate with google servers everytime an object is created
'        Authenticate()
'    End Sub

'    Private Function Authenticate()     'Function that gets authenticates with google servers

'        ' Add the calendar specific scope to the scopes list.
'        scopes.Add(CalendarService.Scope.Calendar)


'        Dim credential As UserCredential
'        Using stream As New FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)
'            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
'                    GoogleClientSecrets.Load(stream).Secrets, scopes, "user", CancellationToken.None,
'                    New FileDataStore("Calendar.VB.Sample")).Result
'        End Using

'        ' Create the calendar service using an initializer instance
'        Dim initializer As New BaseClientService.Initializer()
'        initializer.HttpClientInitializer = credential
'        initializer.ApplicationName = "VB.NET Calendar Sample"
'        service = New CalendarService(initializer)
'        Return 0
'    End Function




'    Sub GetCalendar(MinDate As Date, Optional MaxDate As Date = Nothing)

'        Dim list As IList(Of CalendarListEntry) = service.CalendarList.List().Execute().Items()     'List of all the google calendars the user has
'        Dim EventRequest As ListRequest = service.Events.List(list("0").Id)     'Specifies which google calendar to perform the query

'        EventRequest.TimeMin = MinDate      'Specifies the minimum date to look for in the query
'        MsgBox(list(0).ToString)
'        If Not MaxDate = Nothing Then
'            EventRequest.TimeMax = MaxDate      'Specifies the maximum date to look for in the query
'        End If
'        For Each CalendarEvent As Data.Event In EventRequest.Execute.Items  'For each event in the google calendar add to CalEvents
'            CalEvents.Add(CalendarEvent)
'        Next
'    End Sub


'    Sub UpdateCalendar()
'        Dim CalendarEvent As New Data.Event
'        Dim StartDateTime As New Data.EventDateTime
'        Dim A As Date
'        A = "19/11/2014 12:00"
'        StartDateTime.DateTime = A
'        Dim b As Date
'        b = A.AddHours(2)
'        Dim EndDateTime As New Data.EventDateTime
'        EndDateTime.DateTime = b
'        CalendarEvent.Start = StartDateTime
'        CalendarEvent.End = EndDateTime
'        CalendarEvent.Id = System.Guid.NewGuid.ToString
'        CalendarEvent.Description = "Test"


'        '  Dim Request As New InsertRequest(service, CalendarEvent, service.Events.List(List("0").Id))
'        ''    Request.CreateRequest()
'        '  Request.Execute()

'    End Sub

'End Class