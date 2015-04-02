Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Net

Public Class Calendar
    Sub New(startbutton As Button, onewebbrowser As WebBrowser, otherwebbrowser As WebBrowser)
        webbrowser1 = onewebbrowser
        webbrowser2 = otherwebbrowser
    End Sub
#Region "Events"
    Public Event Status(status As String)
#End Region

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
    Public Structure ClassTime
        Dim startTime As String
        Dim endTime As String
    End Structure
    Public Structure AWeek
        Dim Monday As String
        Dim Tuesday As String
        Dim Wednesday As String
        Dim Thursday As String
        Dim Friday As String
        Dim Saturday As String
        Dim Sunday As String
    End Structure
    Public Structure TermandFinalTime
        Dim currentTerm As String
        Dim termStart As String
        Dim termEnd As String
        Dim finalStart As String
        Dim finalEnd As String
    End Structure
#End Region

#Region "Global Vars"
    Dim bclicked As Boolean = False
    Dim submitTLink As HtmlElement
    Dim studentClasses As New List(Of CalendarEvent)
    Dim thisTerm As New TermandFinalTime
    Dim FinalsWeek As New AWeek
    Dim FirstWeek As New AWeek
    Dim eventBox As New TextBox
    Dim webbrowser1 As WebBrowser
    Dim webbrowser2 As WebBrowser
    Dim stButton As Button
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
        eventBox.AppendText(sb.ToString)
        Return True
    End Function
    Function FinalizeCalendar()
        eventBox.AppendText("END:VCALENDAR")
        File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics").Dispose()
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics", eventBox.Text, False)
        MsgBox("Done! There is now a file on your desktop named my_schedule.ics. Please take that file and upload it to your google calendar.")
        Return True
    End Function
    Function ClassTimes(classTime As String)

        Dim startTime As String = classTime.Split("-")(0)

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


        Dim endTime As String = classTime.Split("-")(1)

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

        Dim finalClassTime As New ClassTime
        finalClassTime.startTime = startTime
        finalClassTime.endTime = endTime
        Return finalClassTime
    End Function
    Function MonthToNumber(month As String)
        Select Case month
            Case "January"
                Return "01"
            Case "February"
                Return "02"
            Case "March"
                Return "03"
            Case "April"
                Return "04"
            Case "May"
                Return "05"
            Case "June"
                Return "06"
            Case "July"
                Return "07"
            Case "August"
                Return "08"
            Case "September"
                Return "09"
            Case "October"
                Return "10"
            Case "November"
                Return "11"
            Case "December"
                Return "12"
            Case Else
                Throw New Exception("ERROR: Month Not Found!")
        End Select
    End Function
    Function FindWeek(theStart As String, week As AWeek)
        Dim month = MonthToNumber(theStart.Split(" ")(0))
        Dim day = Convert.ToInt32(theStart.Split(" ")(1))

        Dim daysinMonth = Date.DaysInMonth(Date.Today.Year, month)
        Dim days As New List(Of String)
        For i = 0 To 6
            If (day + i) > daysinMonth Then
                month = (Convert.ToInt32(month) + 1) Mod 12
            End If
            days.Add(month & "" & (day + i) Mod daysinMonth)
        Next
        week.Saturday = days(0)
        week.Sunday = days(1)
        week.Monday = days(2)
        week.Tuesday = days(3)
        week.Wednesday = days(4)
        week.Thursday = days(5)
        week.Friday = days(6)
        Return True
    End Function
    Function FindCurrentTerm(webbrowser As WebBrowser)
        Dim success As Boolean
        Dim x = Date.Today.Year.ToString
        Dim termRegex As New Regex("((Spring|Winter|Summer|Fall) " & x & " Courses)")
        Dim termMatches As Match = Regex.Match(webbrowser.DocumentText, termRegex.ToString)
        If Not termMatches.Groups(2).Value.Contains("Fall") Then x = Convert.ToInt32(x) - 1
        Dim request As HttpWebRequest = HttpWebRequest.Create("http://blink.ucsd.edu/instructors/resources/academic/calendars/" & x & ".html")

        With request

            .Referer = "http://www.google.com"
            .UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7"
            .KeepAlive = True
            .Method = "GET"
            Dim response As System.Net.HttpWebResponse = .GetResponse
            Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
            Dim dataresponse As String = sr.ReadToEnd

            Dim termInfo As New Regex(".*Fall\sQuarter\sbegins.*\s*[^>]*>([a-zA-Z0-9,\s]*)[^>]*>[^>]*>[^>]*>[^>]*>Instruction\sbegins[^>]*>[^>]*>([a-zA-Z0-9,\s]*)[^>]*>([^>]*>){1,30}Instruction\sends[^>]*>[^>]*>([a-zA-Z0-9,\s]*)[^>]*>[^>]*>[^>]*>[^>]*>Final\sExams[^>]*>[^>]*>(Saturday[a-zA-Z0-9*\s&#;,]*)")

            Dim termInfoMatches As Match = Regex.Match(dataresponse, termRegex.ToString)

            thisTerm.termStart = termInfoMatches.Groups(2).Value
            thisTerm.termEnd = termInfoMatches.Groups(4).Value
            thisTerm.finalStart = System.Web.HttpUtility.HtmlDecode(termInfoMatches.Groups(5).Value.Split(",")(1)).Split("–")(0)
            thisTerm.finalEnd = System.Web.HttpUtility.HtmlDecode(termInfoMatches.Groups(5).Value.Split(",")(1)).Split("–")(1)
            'Saturday** &#8211; Saturday, December 13**&#8211;20<

        End With
        success = FindWeek(thisTerm.termStart, FirstWeek)
        success = FindWeek(thisTerm.finalStart, FinalsWeek)

        Return termMatches.Groups(2).Value

    End Function
#End Region

    Sub Append_Event(evt As CalendarEvent, week As AWeek, Optional ByVal final As Integer = 0)
        Dim time As New ClassTime
        time = ClassTimes(evt.classTime)

        Dim sb As New StringBuilder
        sb.AppendLine("BEGIN:VEVENT")

        Dim weekday As String = ""
        Select Case evt.classDays.Replace(" ", "")
            Case "MO"
                weekday = week.Monday
            Case "TU"
                weekday = week.Tuesday
            Case "WE"
                weekday = week.Wednesday
            Case "TH"
                weekday = week.Thursday
            Case "FR"
                weekday = week.Friday
            Case "SA"
                weekday = week.Saturday
            Case "SU"
                weekday = week.Sunday
        End Select


        sb.AppendLine("DTSTART;TZID=America/Los_Angeles:2015" & weekday & "T" & (time.startTime).Split(":")(0) & evt.classTime.Split("-")(0).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        sb.AppendLine("DTEND;TZID=America/Los_Angeles:2015" & weekday & "T" & (time.endTime) & evt.classTime.Split("-")(1).Replace("p", "").Split(":")(1).Replace("a", "") & "00").Replace("m", "").Replace(" ", "").Replace("Aerica", "America")
        If final = 0 Then
            sb.AppendLine("RRULE:FREQ=WEEKLY;UNTIL=2015" & MonthToNumber(thisTerm.termEnd.Split(",")(1).Trim(" ").Split(" ")(0)) & "" & thisTerm.termEnd.Split(",")(1).Trim(" ").Split(" ")(1) & "T140000Z;BYDAY=" & evt.classDays.Replace(" ", ""))
        End If
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
        eventBox.AppendText(sb.ToString)

    End Sub

    Sub find_classes(source As String)
        If (CreateCalendar()) Then

            'string to split classes by day
            Dim delim As String() = New String(0) {"<h4>"}

            'split the html code by class days
            Dim splitByDays = source.Split(delim, StringSplitOptions.None)

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
                    studentClasses.Add(my_evt)

                    RaiseEvent Status("Found Class: " & className.Replace("	", "") & "from " & classTime & " on " & dayMatches.Groups(1).Value & vbNewLine)
                    'append_events(my_evt)

                Next
            Next
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
                RaiseEvent Status("Found Final: " & className.Replace("	", "") & "from " & classTime & " on " & dayMatches.Groups(1).Value & vbNewLine)
                studentClasses.Add(my_evt)

            Next
        Next

        eventBox.AppendText("END:VCALENDAR")
        File.Create(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics").Dispose()
        '  System.Diagnostics.Process.Start("rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 2")
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Desktop & "/my_schedule.ics", eventBox.Text, False)

        MsgBox("Done! There is now a file on your desktop named my_schedule.ics. Please take that file and upload it to your google calendar.")
    End Sub


End Class
